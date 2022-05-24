using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Api;

public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
{
	public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
	{
		context.Response.StatusCode = 401;
		return Task.CompletedTask;
	}

	public override Task RedirectToAccessDenied(RedirectContext<CookieAuthenticationOptions> context)
	{
		context.Response.StatusCode = 403;
		return Task.CompletedTask;
	}
}
