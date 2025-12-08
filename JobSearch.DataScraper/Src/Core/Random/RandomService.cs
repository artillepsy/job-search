namespace JobSearch.DataScraper.Core.Random;

public class RandomService : IRandomService
{
	public float NextFloat(float minVal, float maxVal)
	{
		var multiplier = System.Random.Shared.NextSingle();
		return minVal + (multiplier * (maxVal - minVal));
	}

	public int NextInt(int minVal, int maxVal)
	{
		return System.Random.Shared.Next(minVal, maxVal);
	}
}