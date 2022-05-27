using ApiLibrary;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/account")]
public class Account : ControllerBase
{
	private readonly ISsoService _ssoService;

	public Account(ISsoService ssoService)
	{
		_ssoService = ssoService;
	}

	[AllowAnonymous]
	[HttpGet]
	[Route("authorize")]
	public async Task<IActionResult> AuthorizeByCode([FromHeader(Name = "code")] string code, [FromHeader(Name = "code_verifier")] string codeVerifier)
	{
		try
		{
			await _ssoService.AuthorizeByCode(HttpContext, code, codeVerifier);
			return new JsonResult(new { success = true });
		}
		catch (Exception ex)
		{
			return Problem(ex.Message);
		}
	}

	[HttpGet]
	[Route("userinfo")]
	public IActionResult UserInformation()
	{
		var subFromClaim = User.FindFirstValue("sub");
		var subFromSession = HttpContext.Session.GetString("sub");
		return new JsonResult(new { success = "userinfo", subFromClaim, subFromSession });
	}

	[AllowAnonymous]
	[HttpGet]
	[Route("anonymous")]
	public IActionResult Anonymous()
	{
		return new JsonResult(new { success = "anonymous" });
	}
}
