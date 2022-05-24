using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Security.Claims;

namespace Api;

// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class CustomTokenValidatorMiddleware
{
	private readonly RequestDelegate _next;

	public CustomTokenValidatorMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public async Task Invoke(HttpContext context)
	{
		var user = context.User;
		if (!string.IsNullOrEmpty(context.User.FindFirstValue("sub")))
		{
			var accessToken = context.Session.GetString("AccessToken");

			if (string.IsNullOrEmpty(accessToken))
			{
				await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				context.Session.Clear();
				context.Response.StatusCode = (int)HttpStatusCode.Conflict;
				return;
			}
			else
			{
				var accessTokenVerified = context.Session.GetString("AccessToken.Verified");
				context.Session.SetString("AccessToken.Verified", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
				if (!string.IsNullOrEmpty(accessTokenVerified))
				{
					var verified = DateTime.ParseExact(accessTokenVerified, "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
					if (verified.AddMinutes(0) < DateTime.UtcNow)
					{
						var apiClient = new HttpClient();
						apiClient.SetBearerToken(accessToken);
						var response = await apiClient.GetAsync("https://localhost:5001/api/sessions/status");
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
							context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
							return;
						}
					}
				}
			}
		}

		await _next(context);
	}
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class CustomTokenValidatorMiddlewareExtensions
{
	public static IApplicationBuilder UseCustomTokenValidatorMiddleware(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<CustomTokenValidatorMiddleware>();
	}
}
