using System.Security.Cryptography;
using System.Text;

namespace JobSearch.DataScraper.Scraping.Services;

public class UrlHashSha1Service : IUrlHashService
{
	public string HashUrl(string id, string url)
	{
		var normalizedUrl = url.ToLowerInvariant().TrimEnd('/');
		var combinedStr = $"{id}|{normalizedUrl}";

		using var sha1 = SHA1.Create();
		
		var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(combinedStr));
		var hashStr = Convert.ToHexStringLower(hash);
		return hashStr;
	}
}