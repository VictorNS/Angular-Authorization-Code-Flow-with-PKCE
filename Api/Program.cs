using Microsoft.AspNetCore.Authentication.Cookies;
using ApiLibrary;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddTransient<ISsoService, SsoService>();
builder.Services.AddSingleton<IdentityServerSettings>(options =>
	options.GetService<IConfiguration>().GetSection("IdentityServer").Get<IdentityServerSettings>());
//builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
		options.Cookie.SameSite = SameSiteMode.None;
		options.Cookie.Name = ".Angular.Authentication";
		options.ExpireTimeSpan = TimeSpan.FromHours(8);
		options.SlidingExpiration = true;
		options.EventsType = typeof(CustomCookieAuthenticationEvents);
	});
builder.Services.AddSingleton<CustomCookieAuthenticationEvents>();
builder.Services.AddAuthorization(options =>
	options.AddPolicy("ApiScope", policy =>
	{
		policy.RequireAuthenticatedUser();
		policy.RequireClaim("sub");
	})
);

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromHours(8);
	options.Cookie.HttpOnly = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.Name = ".Angular.Session";
	options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddCors(options =>
{
	options.AddPolicy("ang", policy =>
	{
		policy.WithOrigins("http://localhost:4200")
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowCredentials();
	});
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ang");
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseCustomTokenValidatorMiddleware();
//app.MapRazorPages();
app.MapControllers().RequireAuthorization("ApiScope");

app.Run();
