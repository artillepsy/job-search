namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.UsaJobs;

public class UsaJobsPageModel
{
	public SearchResult SearchResult { get; set; }

	public override string ToString()
	{
		return $"SearchResult: {SearchResult}";
	}
}

public class SearchResult
{
	public int SearchResultCount { get; set; }
	public int SearchResultCountAll { get; set; }
	public UserArea UserArea { get; set; }
	public List<SearchResultItem> SearchResultItems { get; set; }
	
	public override string ToString()
	{
		return
			$"Number of pages: {UserArea.NumberOfPages}\n" +
			$"Count: {SearchResultCount}\n" +
			$"Total Found: {SearchResultCountAll}\n" +
			string.Join("\n", SearchResultItems.Select((item, index) => $"[{index}] {item}")) + "\n";
	}
}

public class UserArea
{
	public int NumberOfPages { get; set; }
	
}

public class SearchResultItem
{
	public MatchedObjectDescriptor MatchedObjectDescriptor { get; set; }

	public override string ToString()
	{
		return MatchedObjectDescriptor?.ToString() ?? "No Descriptor Available";
	}
}

public class MatchedObjectDescriptor
{
	public string PositionTitle { get; set; }
	public string PositionURI { get; set; }
	public string PositionLocationDisplay { get; set; }
	public string OrganizationName { get; set; }
	public bool RemoteIndicator { get; set; }
	public DateTime PublicationStartDate { get; set; }
	public List<PositionRemuneration> PositionRemuneration { get; set; }

	public override string ToString()
	{
		return
			$"- Title: {PositionTitle}\n" +
			$"- Org: {OrganizationName}\n" +
			$"- Remote: {RemoteIndicator}\n" +
			$"- Location: {PositionLocationDisplay}\n" +
			$"- Date: {PublicationStartDate:yyyy-MM-dd}\n" +
			$"- Salary: {PositionRemuneration}\n" +
			$"- URL: {PositionURI}";
	}
}

public class PositionRemuneration
{
	public string MinimumRange { get; set; }
	public string MaximumRange { get; set; }

	public override string ToString()
	{
		return $"{MinimumRange} - {MaximumRange}";
	}
}