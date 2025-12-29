namespace JobSearch.Data.Entities;

public class JobEntity
{
	public int Id { get; set; }
	
	public string Title { get; set; } = "";
	public string CompanyName { get; set; } = "";
	public string? Location { get; set; }

	public bool IsSalaryVisible { get; set; }
	public string Salary { get; set; } = "";

	public string Website { get; set; } = "";
	public string WebsiteSpecificId { get; set; } = "";
	
	public string Url { get; set; } = "";

	public DateTime CreatedAt { get; set; }
}