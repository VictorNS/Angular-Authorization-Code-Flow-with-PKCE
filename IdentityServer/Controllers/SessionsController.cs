using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using static Duende.IdentityServer.IdentityServerConstants;

namespace IdentityServer.Controllers
{
	[Route("api/sessions")]
	[Authorize(LocalApi.PolicyName)]
	[ApiController]
	public class SessionsController : ControllerBase
	{
		private readonly ISessionManagementService _sessionManagementService;

		public SessionsController(ISessionManagementService sessionManagementService)
		{
			_sessionManagementService = sessionManagementService;
		}

		[HttpGet("status/{sid}")]
		public async Task<IActionResult> GetSessionStatus(string sid)
		{
			var sub = User.FindFirstValue("sub");
			var q = new SessionQuery
			{
				CountRequested = 0,
				RequestPriorResults = false,
				ResultsToken = null,
				SubjectId = sub,
				DisplayName = sub, // If you think this code is strange, visit:
				SessionId = sub,   // https://github.com/DuendeSoftware/IdentityServer/blob/main/src/IdentityServer/Stores/InMemory/InMemoryServerSideSessionStore.cs#L171
			};
			var result = await _sessionManagementService.QuerySessionsAsync(q);
			var session = result.Results.FirstOrDefault(x => x.SessionId == sid);
			if (session == null)
				return Ok("");

			return Ok(session.SessionId);
		}

		[HttpGet("tryfind")]
		public async Task<IActionResult> GetFistSessionBySubject()
		{
			var sub = User.FindFirstValue("sub");
			var q = new SessionQuery
			{
				CountRequested = 0,
				RequestPriorResults = false,
				ResultsToken = null,
				SubjectId = sub,
				DisplayName = sub, // If you think this code is strange, visit:
				SessionId = sub,   // https://github.com/DuendeSoftware/IdentityServer/blob/main/src/IdentityServer/Stores/InMemory/InMemoryServerSideSessionStore.cs#L171
			};
			var result = await _sessionManagementService.QuerySessionsAsync(q);
			var session = result.Results.FirstOrDefault(x => x.SubjectId == sub);
			if (session == null)
				return Ok("");

			return Ok(session.SessionId);
		}
	}
}
