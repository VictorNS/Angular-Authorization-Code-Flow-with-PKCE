using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using IdentityModel;

namespace IdentityServer;

public static class Config
{
	public static IEnumerable<IdentityResource> IdentityResources =>
		new List<IdentityResource>
		{
			new IdentityResources.OpenId(),
			new IdentityResources.Profile(),
			new IdentityResource()
			{
				Name = "verification",
				UserClaims = new List<string>
				{
					JwtClaimTypes.Email,
					JwtClaimTypes.EmailVerified
				}
			}
		};

	public static IEnumerable<ApiScope> ApiScopes =>
		new List<ApiScope>
		{
			new ApiScope(IdentityServerConstants.LocalApi.ScopeName),
			new ApiScope("api1", "MyAPI")
		};

	public static IEnumerable<Client> Clients =>
		new List<Client>
		{
            new Client
			{
				ClientId = "ang",
				ClientName = "Angular Client",
				AllowedGrantTypes = GrantTypes.Code,
				DeviceCodeLifetime = 8*60*60,
				RequireClientSecret = false,

				RedirectUris =           { "http://localhost:4200/oauth/callback" },
				PostLogoutRedirectUris = { "http://localhost:4200/login" },
				AllowedCorsOrigins =     { "http://localhost:4200" },

				AllowedScopes =
				{
					IdentityServerConstants.StandardScopes.OpenId,
					IdentityServerConstants.StandardScopes.Profile,
					"api1",
					IdentityServerConstants.LocalApi.ScopeName
				}
			},
		};
}