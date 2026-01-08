using JobSearch.Api.Services;
using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

if (app.Environment.IsDevelopment())
{
	app.UseHttpsRedirection();
}

// swagger + openApi
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.UseSwaggerUI(o =>
	{
		o.SwaggerEndpoint("/openapi/v1.json", "API v1");
		o.RoutePrefix = "swagger"; // UI at /swagger
	});
}

// map api endpoints
app.MapControllers();

// spa proxy, exclude swagger, api and openapi
app.MapWhen(ctx =>
		!ctx.Request.Path.StartsWithSegments("/api", StringComparison.OrdinalIgnoreCase) &&
		!ctx.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase) &&
		!ctx.Request.Path.StartsWithSegments("/openapi", StringComparison.OrdinalIgnoreCase),
	spaApp =>
	{
		spaApp.UseSpa(spa =>
		{
			if (app.Environment.IsDevelopment())
				spa.UseProxyToSpaDevelopmentServer("http://localhost:4200");
		});
	});

// Migrate DB (dev only or guarded)
if (app.Environment.IsDevelopment())
{
	using var scope = app.Services.CreateScope();
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	
	try
	{
		await db.Database.MigrateAsync();
	}
	catch (Exception ex)
	{
		app.Logger.LogError(ex, "Database migration failed");
	}
}

app.Run();
