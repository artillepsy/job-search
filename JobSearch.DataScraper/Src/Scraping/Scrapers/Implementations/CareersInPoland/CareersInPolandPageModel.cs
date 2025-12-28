using System.Text.Json.Serialization;

namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.CareersInPoland;

public class CareersInPolandPageModel
{
	[JsonPropertyName("jobOffers")]
	public JobOffers JobOffers { get; set; }
	
	public override string ToString() => $"JobOffers: {JobOffers} ";
}

public class JobOffers
{
	[JsonPropertyName("pagination")]
	public Pagination Pagination { get; set; }
	
	public override string ToString() => $"Pagination: {Pagination}\n\n";
}

public class Pagination
{
	[JsonPropertyName("current_page")]
	public int CurrentPage { get; set; }
	[JsonPropertyName("last_page")]
	public int LastPage { get; set; }
	[JsonPropertyName("next_page_url")]
	public string NextPageUrl { get; set; } = "";
	[JsonPropertyName("total")]
	public int Total { get; set; }

	[JsonPropertyName("data")]
	public List<JobData> Data { get; set; }

	[JsonIgnore]
	public bool IsLastPage => CurrentPage == LastPage;
	
	public override string ToString()
	{
		return
			$"CurrentPage: {CurrentPage}\n" +
			$"LastPage: {LastPage}\n" +
			$"NextPageUrl: {NextPageUrl}\n" +
			$"Total: {Total}\n" +
			string.Join("\n", Data.Select((item, index) => $"[{index}] {item}")) + "\n";
	}
}

public class JobData
{
	[JsonPropertyName("title")]
	public string Title { get; set; }
	[JsonPropertyName("date")]
	public DateTime Date { get; set; }

	[JsonPropertyName("work_time")]
	public string WorkTime { get; set; }
	
	[JsonPropertyName("employer_name")]
	public string EmployerName { get; set; }
	[JsonPropertyName("salary")]
	public string Salary { get; set; }
	
	[JsonPropertyName("locations")]
	public List<JobLocation> Locations { get; set; }
	
	public override string ToString()
	{
		return
			$"- Title: {Title}\n" +
			$"- Date: {Date}\n" +
			$"- WorkTime: {WorkTime}\n" +
			$"- EmployerName: {EmployerName}\n" +
			$"- Salary: {Salary}\n" +
			string.Join("\n", Locations);
	}
}

public class JobLocation
{
	[JsonPropertyName("url")]
	public string RelativeUrl { get; set; }
	
	[JsonIgnore]
	public string FullUrl => $"https://careersinpoland.com{RelativeUrl}"; 

	[JsonPropertyName("location")]
	public string Location { get; set; }

	public override string ToString()
	{
		return
			$"- - RelativeUrl: {RelativeUrl}\n" +
			$"- - FullUrl: {FullUrl}\n" +
			$"- - Location: {Location}";
	}
}