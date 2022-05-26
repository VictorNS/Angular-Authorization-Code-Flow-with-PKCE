using IdentityServerHost;
using Serilog;

namespace IdentityServer;

internal static class HostingExtensions
{
	public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
	{
		builder.Services.AddRazorPages();
		builder.Services.AddControllers();

		builder.Services.AddIdentityServer(options =>
			{
				//options.ServerSideSessions.UserDisplayNameClaimType = IdentityModel.JwtClaimTypes.Email;
			})
			.AddServerSideSessions()
			.AddInMemoryIdentityResources(Config.IdentityResources)
			.AddInMemoryApiScopes(Config.ApiScopes)
			.AddInMemoryClients(Config.Clients)
			.AddTestUsers(TestUsers.Users);

		builder.Services.AddAuthentication();
		builder.Services.AddLocalApiAuthentication();

		return builder.Build();
	}

	public static WebApplication ConfigurePipeline(this WebApplication app)
	{
		app.UseSerilogRequestLogging();
		if (app.Environment.IsDevelopment())
		{
			app.UseDeveloperExceptionPage();
		}

		app.UseStaticFiles();
		app.UseRouting();

		app.UseIdentityServer();

		app.UseAuthorization();
		app.MapRazorPages().RequireAuthorization();
		app.MapControllers();

		return app;
	}
}