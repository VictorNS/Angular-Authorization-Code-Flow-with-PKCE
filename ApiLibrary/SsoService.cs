using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Text.Json;

namespace ApiLibrary
{
	public interface ISsoService
	{
		Task AuthorizeByCode(HttpContext currentContext, string code, string codeVerifier);
	}

	public class SsoService : ISsoService
	{
		private const string ClientCallbackPath = "/oauth/callback";
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly IdentityServerSettings _settings;

		public SsoService(IHttpClientFactory httpClientFactory, IdentityServerSettings settings)
		{
			_httpClientFactory = httpClientFactory;
			_settings = settings;
		}

		public async Task AuthorizeByCode(HttpContext currentContext, string code, string codeVerifier)
		{
			// discover endpoints from metadata
			var httpClient = _httpClientFactory.CreateClient();
			var disco = await httpClient.GetDiscoveryDocumentAsync(_settings.Url);
			if (disco.IsError)
			{
				throw new Exception(disco.Error);
			}

			// request token
			var tokenResponse = await httpClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = _settings.ClientId,
				Code = code,
				CodeVerifier = codeVerifier,
				RedirectUri = _settings.ClientUrl + ClientCallbackPath
			});

			if (tokenResponse.IsError)
			{
				throw new Exception(tokenResponse.Error + " :: " + tokenResponse.ErrorDescription);
			}

			// request user info
			var apiClient = _httpClientFactory.CreateClient();
			apiClient.SetBearerToken(tokenResponse.AccessToken);

			var response = await apiClient.GetAsync(disco.UserInfoEndpoint);
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(response.ToString());
			}

			try
			{
				currentContext.Session.SetString("AccessToken", tokenResponse.AccessToken);
				//HttpContext.Session.SetString("RefreshToken", tokenResponse.RefreshToken);
			}
			catch
			{
				throw new Exception("Valid AccessToken is expected.");
			}

			try
			{
				var token = tokenResponse.IdentityToken;
				var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
				var jwtToken = handler.ReadJwtToken(token);
				var sid = jwtToken.Claims.First(claim => claim.Type == JwtClaimTypes.SessionId).Value;
				currentContext.Session.SetString("SessionId", sid);
			}
			catch
			{
				throw new Exception("Valid IdentityToken is expected.");
			}

			var content = await response.Content.ReadAsStringAsync();
			var parsed = JsonDocument.Parse(content);
			var sub = "";
			foreach (var el in parsed.RootElement.EnumerateObject())
			{
				var name = el.Name;
				var value = el.Value.GetString();

				if (!string.IsNullOrWhiteSpace(value))
				{
					if (name == JwtClaimTypes.Subject)
						sub = value;
					currentContext.Session.SetString(name, value);
				}
			}

			if (string.IsNullOrWhiteSpace(sub))
			{
				throw new Exception($"JwtClaimTypes.Subject ({JwtClaimTypes.Subject}) claim is expected");
			}

			// sign in
			var claims = new List<Claim>
			{
				new Claim(JwtClaimTypes.Subject, sub)
			};

			var claimsPrincipal = new ClaimsPrincipal(
				new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));

			var authProperties = new AuthenticationProperties
			{
				AllowRefresh = true,
				// Refreshing the authentication session should be allowed.

				//ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
				// The time at which the authentication ticket expires. A 
				// value set here overrides the ExpireTimeSpan option of 
				// CookieAuthenticationOptions set with AddCookie.

				//IsPersistent = true,
				// Whether the authentication session is persisted across 
				// multiple requests. When used with cookies, controls
				// whether the cookie's lifetime is absolute (matching the
				// lifetime of the authentication ticket) or session-based.

				//IssuedUtc = <DateTimeOffset>,
				// The time at which the authentication ticket was issued.

				//RedirectUri = <string>
				// The full path or absolute URI to be used as an http 
				// redirect response value.
			};

			await currentContext.SignInAsync(
				CookieAuthenticationDefaults.AuthenticationScheme,
				claimsPrincipal,
				authProperties);

			return;
		}
	}
}
