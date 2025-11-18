using JobSearch.Server.Models;
using JobSearch.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace JobSearch.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly IPasswordHasher<UserModel> _hasher;
	private readonly ITokenService _tokenService;

	public record RegisterDto(string Username, string Password);
	public record LoginDto(string Username, string Password);

	public UsersController(AppDbContext db, IPasswordHasher<UserModel> hasher, ITokenService tokenService)
	{
		_db = db;
		_hasher = hasher;
		_tokenService = tokenService;
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
	
	//??
	[HttpGet("{username}")]
	public async Task<IActionResult> GetByUsername(string username) 
		=> Ok($"User {username} has been successfully created");

	[AllowAnonymous]
	[HttpPost("login")]
	public async Task<ActionResult<object>> Login([FromBody] LoginDto dto)
	{
		if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
			return BadRequest("Username and password are required");

		var username = dto.Username.Trim().ToLowerInvariant();
		
		var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == username);
		if (user is null)
			return Unauthorized(new { error = "Username not found" });

		var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
		if (result == PasswordVerificationResult.Failed)
			return Unauthorized(new { error = "Wrong password" });

		if (result == PasswordVerificationResult.SuccessRehashNeeded)
		{
			user.PasswordHash = _hasher.HashPassword(user, dto.Password);
			await _db.SaveChangesAsync();
		}

		return Ok(new
		{
			ok = true,
			userId = user.Id,
			username = user.Username,
			token = _tokenService.GenerateDevToken(user.Username),
		});
	}
}