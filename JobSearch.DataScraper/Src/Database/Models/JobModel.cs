namespace JobSearch.DataScraper.Database.Models;

public class JobModel
{
	public int Id { get; set; }
	
	public string Title { get; set; } = "";
	public string CompanyName { get; set; } = "";
	public string? Location { get; set; }

	public bool IsSalaryVisible { get; set; }
	public decimal? SalaryFrom { get; set; }
	public decimal? SalaryTo { get; set; }

	public string SourceWebsite { get; set; } = "";
	public string Url { get; set; } = "";
	public DateTime CreatedAt { get; set; }
}