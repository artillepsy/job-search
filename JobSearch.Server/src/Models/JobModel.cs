namespace JobSearch.Server.Models;

public class JobModel
{
	public int Id { get; set; }
	public string Title { get; set; } = "";
	public string CompanyName { get; set; } = "";
	public decimal? Salary { get; set; }
	public bool IsSalaryVisible { get; set; }
	public string Location { get; set; }
	public DateTime CreatedAt { get; set; }
}