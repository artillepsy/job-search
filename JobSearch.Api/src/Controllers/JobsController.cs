using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
	private readonly AppDbContext _db;

	public record JobSearchDto(string Title);
	public record JobPostingDto(
		int id,
		string Title, 
		string CompanyName, 
		decimal? SalaryMin,
		decimal? SalaryMax,
		string? Currency,
		string Location);
	
	public JobsController(AppDbContext db)
	{
		_db = db;
	}
	
	// todo: paged results
	// todo: preload next page
	// todo: max results limit (in config file)
	//[AllowAnonymous]
	[HttpGet("get-all")]
	public async Task<ActionResult<IEnumerable<JobEntity>>> GetJobs(
		[FromQuery] int pageNumber = 1, 
		[FromQuery] int pageSize = 100)
	{
		pageNumber = pageNumber < 1 ? 1 : pageNumber;
		pageSize = pageSize > 100 ? 100 : pageSize;
		
		IQueryable<JobEntity> query = _db.Jobs;
		var totalRecords = await query.CountAsync();
		var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);
		
		var jobs = await query
			.OrderByDescending(j => j.CreatedAt) // change order based on filters
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.Select(j => new JobPostingDto(
				j.Id,
				j.Title, 
				j.CompanyName, 
				j.SalaryMin,
				j.SalaryMax,
				j.Currency,
				j.Location))
			.ToListAsync();
		
		Console.WriteLine("========================================");
		Console.WriteLine($"SERVER LOG: {DateTime.Now:T}");
		Console.WriteLine($"Request: Page {pageNumber}, Size {pageSize}");
		Console.WriteLine($"Database: Total Records = {totalRecords}");
		Console.WriteLine($"Result: Returning {jobs.Count} jobs");
    
		foreach (var job in jobs)
		{
			Console.WriteLine($" -> [JOB] {job.Title} at {job.CompanyName}");
		}
		Console.WriteLine("========================================");
		
		return Ok(new {
			TotalPages = totalPages,
			PageNumber = pageNumber,
			PageSize = pageSize,
			
			TotalRecords = totalRecords,
			ReturnRecords = jobs.Count,
			
			Jobs = jobs
		});
	}
	
	//[AllowAnonymous]
	[HttpGet("get")]
	public async Task<ActionResult<IEnumerable<JobEntity>>> GetJobs([FromQuery] JobSearchDto dto)
	{
		IQueryable<JobEntity> query = _db.Jobs;
		
		if (!string.IsNullOrWhiteSpace(dto.Title))
		{
			query = query.Where(j => j.Title.Contains(dto.Title));
		}
		
		var jobs = await query.ToListAsync();
		return Ok(jobs);
	}

	//make it dev only + add date randomizer
	//todo: add user token as a required field, remove [allowAnonymous]
	/*[AllowAnonymous]
	[HttpPost("post")]
	public async Task<ActionResult> PostJob([FromBody] JobPostingDto[] dtoList)
	{
		var jobs = new List<JobEntity>();
		
		foreach (var dto in dtoList)
		{
			var job = new JobEntity()
			{
				Title = dto.Title,
				CompanyName = dto.CompanyName,
				Salary = dto.Salary,
				Location = dto.Location,
				CreatedAt = DateTime.UtcNow,
			};
			jobs.Add(job);
		}
		
		await _db.Jobs.AddRangeAsync(jobs);

		try
		{
			await _db.SaveChangesAsync();
		}
		catch (DbUpdateException ex)
		{
			return Conflict(new { error = $"Unable to post a job, error: {ex.Message}" });
		}

		return Ok();
	}*/
}