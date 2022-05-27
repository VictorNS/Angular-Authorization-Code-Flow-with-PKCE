using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace ApiLibrary;

public class CustomTokenValidatorMiddleware
{
	private const string SessionApiPath = "/api/sessions/status";
	private readonly RequestDelegate _next;

	public CustomTokenValidatorMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context, IHttpClientFactory httpClientFactory, IdentityServerSettings settings)
	{
		var user = context.User;
		if (!string.IsNullOrEmpty(context.User.FindFirstValue(JwtClaimTypes.Subject)))
		{
			var accessToken = context.Session.GetString("AccessToken");

			if (string.IsNullOrEmpty(accessToken))
			{
				await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				context.Session.Clear();
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				return;
			}
			else
			{
				var accessTokenVerified = context.Session.GetString("AccessToken.Verified");
				context.Session.SetString("AccessToken.Verified", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
				if (!string.IsNullOrEmpty(accessTokenVerified))
				{
					var verified = DateTime.ParseExact(accessTokenVerified, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
					if (verified.AddMinutes(1) < DateTime.UtcNow)
					{
						var httpMessage = new HttpRequestMessage(HttpMethod.Get, settings.Url + SessionApiPath);
						httpMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

						var httpClient = httpClientFactory.CreateClient();
						var response = await httpClient.SendAsync(httpMessage);
						var forbidden = !response.IsSuccessStatusCode;

						if (!forbidden)
						{
							string sessionId = await response.Content.ReadAsStringAsync();
							forbidden = string.IsNullOrEmpty(sessionId);
						}

						if (forbidden)
						{
							await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
							context.Session.Clear();
							context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
							return;
						}
					}
				}
			}
		}

		await _next(context);
	}
}

public static class CustomTokenValidatorMiddlewareExtensions
{
	public static IApplicationBuilder UseCustomTokenValidatorMiddleware(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<CustomTokenValidatorMiddleware>();
	}
}
