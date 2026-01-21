using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobsController(AppDbContext db) : ControllerBase
{
	public record JobSearchDto(
		string? Title, 
		string? Location, 
		bool? IsSalaryVisible,
		bool? IsRemote
		// add salary range and currency (?)
	);

	private record JobPostingDto(
		int id,
		string Title, 
		string CompanyName, 
		decimal? SalaryMin,
		decimal? SalaryMax,
		string? Currency,
		string Location,
		DateTime CreatedAt);

	// todo: preload next page
	[HttpGet("get-all")]
	public async Task<ActionResult<IEnumerable<JobEntity>>> GetJobs(
		[FromQuery] int pageNumber = 1, 
		[FromQuery] int pageSize = 100)
	{
		pageNumber = pageNumber < 1 ? 1 : pageNumber;
		pageSize = pageSize > 100 ? 100 : pageSize;
		
		IQueryable<JobEntity> query = db.Jobs;
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
				j.Location,
				j.CreatedAt))
			.ToListAsync();
		
		return Ok(new {
			TotalPages = totalPages,
			PageNumber = pageNumber,
			PageSize = pageSize,
			
			TotalRecords = totalRecords,
			ReturnRecords = jobs.Count,
			
			Jobs = jobs
		});
	}
	
	[HttpGet("get")]
	public async Task<ActionResult<IEnumerable<JobEntity>>> GetJobs([FromQuery] JobSearchDto dto)
	{
		IQueryable<JobEntity> query = db.Jobs;
		
		if (!string.IsNullOrWhiteSpace(dto.Title)) // Title or Company name
		{
			string search = dto.Title.ToLower();
			query = query.Where(j => j.Title.ToLower().Contains(search) || j.CompanyName.ToLower().Contains(search));
		}
		
		if (!string.IsNullOrWhiteSpace(dto.Location)) // Location
		{
			query = query.Where(j => j.Location != null && j.Location.ToLower().Contains(dto.Location.ToLower()));
		}
		
		if (dto.IsSalaryVisible.HasValue) // Is salary visible
		{
			query = dto.IsSalaryVisible.Value 
				? query.Where(j => j.SalaryMin != null) 
				: query.Where(j => j.SalaryMin == null);
		}
		
		if (dto.IsRemote.HasValue) // Is remote
		{
			query = query.Where(j => j.IsRemote == dto.IsRemote.Value); 
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