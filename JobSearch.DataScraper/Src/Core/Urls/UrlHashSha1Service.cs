using System.Security.Cryptography;
using System.Text;

namespace JobSearch.DataScraper.Core.Urls;

public class UrlHashSha1Service : IUrlHashService
{
	public string HashUrl(string id, string url)
	{
		var normalizedUrl = url.ToLowerInvariant().TrimEnd('/');
		var combinedStr = $"{id}{url}";

		using var sha1 = SHA1.Create();
		
		var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(combinedStr));
		return Convert.ToHexStringLower(hash);
	}
}