using JobSearch.Data;
using JobSearch.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobSearch.Api.Controllers;

//[Authorize]
[ApiController]
[Route("api/[controller]")]
public class JobsController : ControllerBase
{
	private readonly AppDbContext _db;
	private readonly JobSettings _jobSettings;
	private int i = 10;
	public class JobSettings
	{
		public int PageSizeMax { get; set; }
	}
	
	public record JobSearchDto(
		string? Title, 
		string? Location, 
		bool? IsSalaryVisible,
		bool? IsRemote,
		
		int PageNumber = 1,
		int PageSize = 20
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
		bool IsRemote,
		DateTime CreatedAt);

	public JobsController(AppDbContext db , IConfiguration config)
	{
		_db = db;
		_jobSettings = config.GetSection("JobSettings").Get<JobSettings>() 
		               ?? throw new Exception("JobSettings section not found in appsettings.json");
	}

	// To get all jobs, dto with default params can be sent
	[HttpGet("get")]
	public async Task<ActionResult<IEnumerable<JobEntity>>> GetJobs([FromQuery] JobSearchDto dto)
	{
		int pageNumber = dto.PageNumber < 1 ? 1 : dto.PageNumber;
		int pageSize = dto.PageSize > _jobSettings.PageSizeMax ? _jobSettings.PageSizeMax : dto.PageSize;
		
		IQueryable<JobEntity> query = _db.Jobs;
		
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
		
		var totalRecords = await query.CountAsync();
		var totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

		var jobs = await query
			.OrderByDescending(j => j.CreatedAt)
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
				j.IsRemote,
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
}