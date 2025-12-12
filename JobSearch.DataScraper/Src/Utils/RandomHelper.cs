namespace JobSearch.DataScraper.Utils;

public static class RandomHelper
{
	public static float NextFloat(float minVal, float maxVal)
	{
		var multiplier = System.Random.Shared.NextSingle();
		return minVal + (multiplier * (maxVal - minVal));
	}

	public static int NextInt(int minVal, int maxVal)
	{
		return System.Random.Shared.Next(minVal, maxVal);
	}
}