using JobSearch.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
	private readonly AppDbContext _context;

	public UsersController(AppDbContext context)
	{
		_context = context;
	}

	[HttpGet("exists")]
	public async Task<ActionResult<object>> Exists([FromQuery] string username)
	{
		if (string.IsNullOrWhiteSpace(username))
			return BadRequest("username is required");

		var exists = await _context.Users.AnyAsync(u => EF.Functions.ILike(u.Username, username));

		return Ok(new { exists });
	}
}