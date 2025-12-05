using JobSearch.DataScraper;
using JobSearch.DataScraper.Services.Core.Factory;
using JobSearch.DataScraper.Services.Utils;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//db + ef setup
builder.Services.AddDbContext<AppDbContext>(opt =>
	opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
		.UseSnakeCaseNamingConvention());

builder.Services.AddSingleton<IScraperFactory, ScraperFactory>();

foreach (var (name, binding) in ScraperUtils.Bindings)
{
	builder.Services.AddScoped(binding.Type);
	builder.Services.AddScoped(binding.ConfigType);
}

// mvc + swagger
builder.Services.AddControllers(); 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// misc
builder.Services.AddRouting(o => o.LowercaseUrls = true);

var app = builder.Build();

app.UseHttpsRedirection();

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