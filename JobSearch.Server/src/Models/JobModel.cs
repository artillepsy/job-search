namespace JobSearch.Server.Models;

public class JobModel
{
	public int Id { get; set; }
	public string Title { get; set; } = "";
	//[Column("company_name")]
	public string CompanyName { get; set; } = "";
	public string Location { get; set; } = "";
	public string? Salary { get; set; } 
	public string? Website { get; set; } // New field from DB
	public string? Url { get; set; } // New field from DB
	//[Column("created_at")]
	public DateTime CreatedAt { get; set; }
}