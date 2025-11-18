using JobSearch.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
	private readonly AppDbContext _db;

	public record JobSearchDto(string Title);
	public record JobPostingDto(string Title, string CompanyName, decimal Salary);
	
	public JobsController(AppDbContext db)
	{
		_db = db;
	}
	
	[AllowAnonymous]
	[HttpGet("get-all")]
	public async Task<ActionResult<IEnumerable<JobModel>>> GetJobs()
	{
		IQueryable<JobModel> query = _db.Jobs;
		
		var jobs = await query.ToListAsync();
		
		Console.WriteLine(
			string.Join(" | ", jobs.Select(j =>
				$"Id={j.Id}, Title={j.Title}, Company={j.CompanyName}, Salary={j.Salary}"
			))
		);
		
		return Ok(jobs);
	}
	
	[AllowAnonymous]
	[HttpGet("get")]
	public async Task<ActionResult<IEnumerable<JobModel>>> GetJobs([FromQuery] JobSearchDto dto)
	{
		IQueryable<JobModel> query = _db.Jobs;
		
		if (!string.IsNullOrWhiteSpace(dto.Title))
		{
			query = query.Where(j => j.Title.Contains(dto.Title));
		}
		
		var jobs = await query.ToListAsync();
		return Ok(jobs);
	}

	//todo: add user token as a required field, remove [allowAnonymous]
	[AllowAnonymous]
	[HttpPost("post")]
	public async Task<ActionResult> PostJob([FromBody] JobPostingDto dto)
	{
		var job = new JobModel()
		{
			Title = dto.Title,
			CompanyName = dto.CompanyName,
			Salary = dto.Salary,
		};

		await _db.Jobs.AddAsync(job);

		try
		{
			await _db.SaveChangesAsync();
		}
		catch (DbUpdateException ex)
		{
			return Conflict(new { error = $"Unable to post a job, error: {ex.Message}" });
		}

		return Ok();
	}
}