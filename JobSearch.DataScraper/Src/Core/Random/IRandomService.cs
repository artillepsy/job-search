namespace JobSearch.DataScraper.Core.Random;

public interface IRandomService
{
	public float NextFloat(float minVal, float maxVal);
	public int NextInt(int minVal, int maxVal);
}