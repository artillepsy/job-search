namespace JobSearch.DataScraper.Services.Configuration.Scrapers;

public class CareersInPolandConfig
{
	//todo: max retries
	public string Url { get; set; } = "";
	public float PingIntervalSecMin { get; set; }
	public float PingIntervalSecMax { get; set; }

	public override string ToString() =>
		$"url: {Url}, ping interval min: {PingIntervalSecMin}, ping interval max: {PingIntervalSecMax}";
}