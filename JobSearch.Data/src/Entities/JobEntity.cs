namespace JobSearch.Data.Entities;

public class JobEntity
{
	public int Id { get; set; }
	
	public string Title { get; set; } = "";
	public string CompanyName { get; set; } = "";
	public string? Location { get; set; }

	public decimal? SalaryMin { get; set; }
	public decimal? SalaryMax { get; set; }
	public string? Currency { get; set; }

	public string Website { get; set; } = "";
	
	public string Url { get; set; } = "";

	public DateTime CreatedAt { get; set; }
}