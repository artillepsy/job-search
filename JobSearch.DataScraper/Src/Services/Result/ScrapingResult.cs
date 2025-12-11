namespace JobSearch.DataScraper.Services.Result;

public class ScrapingResult
{
	public bool IsSuccess { get; set; }
	public int RecordsScraped { get; set; }
	public string Error { get; set; } = "";
	
	public TimeSpan Duration { get; set; }
	public DateTime StartedAt { get; set; }
	public DateTime? FinishedAt { get; set; }
	
	public override string ToString() =>
		$"Success: {IsSuccess}\n" +
		$"Records: {RecordsScraped}\n" +
		$"Duration: {Duration}\n" +
		$"Started at: {StartedAt}\n" +
		$"FinishedAt: {FinishedAt}\n" + 
		$"Error: {Error}";
}