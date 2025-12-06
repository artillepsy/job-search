namespace JobSearch.DataScraper.Services.ConfigurationModels;

public class CareersInPolandConfigModel
{
	//todo: max retries
	public string Url { get; set; }
	public float PingIntervalMin { get; set; }
	public float PingIntervalMax { get; set; }

	public override string ToString() =>
		$"url: {Url}, ping interval min: {PingIntervalMin}, ping interval max: {PingIntervalMax}";
}