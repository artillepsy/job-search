using JobSearch.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace JobSearch.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly IPasswordHasher<UserModel> _hasher;

	public record RegisterDto(string Username, string Password);
	public record LoginDto(string Username, string Password);

	public UsersController(AppDbContext db, IPasswordHasher<UserModel> hasher)
	{
		_db = db;
		_hasher = hasher;
	}

	[AllowAnonymous]
	[HttpPost("register")]
	public async Task<ActionResult> Register([FromQuery] RegisterDto dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
			return BadRequest("Username and password are required");

		var username = dto.Username.Trim();
		
		var exists = await _db.Users.AnyAsync(u => EF.Functions.ILike(u.Username, username));
		if (exists)
			return Conflict(new { error = "username is already taken" });

		var user = new UserModel() { Username = username };

		user.PasswordHash = _hasher.HashPassword(user, dto.Password);
		await _db.Users.AddAsync(user);

		try
		{
			await _db.SaveChangesAsync();
		}
		catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
		{
			return Conflict(new { error = "username is already taken" });
		}

		return CreatedAtAction(nameof(GetByUsername), new { username = user.Username }, new { user.Id, user.Username });

	}
	
	[HttpGet("{username}")]
	public async Task<IActionResult> GetByUsername(string username) 
		=> Ok($"User {username} has been successfully created");

	[AllowAnonymous]
	[HttpGet("login")]
	public async Task<ActionResult<object>> Login([FromQuery] LoginDto dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Username.ToLower()))
			return BadRequest("Username is required");

		if (string.IsNullOrWhiteSpace(dto.Password))
			return BadRequest("Password is required");

		var user = await _db.Users.FindAsync(dto.Username.ToLower());

		if (user is null)
			return Ok(new { Error = "wrong password" });

		var isSamePassword = user.PasswordHash.Equals(dto.Password);

		return Ok(new { isSamePassword });
	}
}