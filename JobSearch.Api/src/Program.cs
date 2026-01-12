using JobSearch.Api.Services;
using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(policy =>
	{
		policy.AllowAnyOrigin() // For testing, allow everything. 
			.AllowAnyHeader() // For production, replace with your specific SWA URL.
			.AllowAnyMethod();
	});
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

// MUST be before app.MapControllers
app.UseCors();

// map api endpoints
app.MapControllers();

app.Run();
