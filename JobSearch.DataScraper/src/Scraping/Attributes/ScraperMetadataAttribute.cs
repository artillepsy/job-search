namespace JobSearch.DataScraper.Scraping.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class ScraperMetadataAttribute : Attribute
{
	public string SectionName { get; }

	public ScraperMetadataAttribute(string sectionName)
	{
		SectionName = sectionName.ToLowerInvariant();
	}
}