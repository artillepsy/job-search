using JobSearch.Api.Services;
using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		/*policy.WithOrigins(
				"http://localhost:4200" // For local testing
				// azure swa url
			)*/
		policy.AllowAnyOrigin() // temporary, for test only
			.AllowAnyHeader() // For production, replace with your specific SWA URL.
			.AllowAnyMethod();
	});
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
	options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
	options.KnownIPNetworks.Clear();
	options.KnownProxies.Clear();
});

//db + ef setup
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"), 
			o => o.MigrationsHistoryTable("__EFMigrationsHistory")) // Forces the default name
		.UseSnakeCaseNamingConvention());

// mvc + swagger
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// misc
builder.Services.AddRouting(o => o.LowercaseUrls = true);
builder.Services.AddScoped<IPasswordHasher<UserEntity>, PasswordHasher<UserEntity>>();
builder.Services.AddSingleton<ITokenService, DevTokenService>();

var app = builder.Build();
app.UseForwardedHeaders(); // Always first when behind a proxy (Azure Container Apps)
app.UseRouting(); // Must come first to handle the request path
app.UseCors(); // Must come before MapOpenApi/Controllers to handle Preflight (OPTIONS) requests

// Add Swagger UI to test it in Production
app.MapOpenApi();
app.UseSwaggerUI(o =>
{
	o.SwaggerEndpoint("/openapi/v1.json", "API v1");
	o.RoutePrefix = "swagger"; 
});

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

// map api endpoints
app.MapControllers();
app.Run();
