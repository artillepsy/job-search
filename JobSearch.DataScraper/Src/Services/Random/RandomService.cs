namespace JobSearch.DataScraper.Services.Random;

public class RandomService : IRandomService
{
	public float NextFloat(float minVal, float maxVal)
	{
		var multiplier = System.Random.Shared.NextSingle();
		return minVal + (multiplier * (maxVal - minVal));
	}
}