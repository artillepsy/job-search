using System.Text.Json.Serialization;
using JobSearch.DataScraper.Utils;

namespace JobSearch.DataScraper.Scraping.Scrapers.Implementations.Arbeitnow;

public class ArbeitnowPageModel
{
    [JsonPropertyName("data")]
    public List<JobData> Data { get; set; }

    [JsonPropertyName("links")]
    public PageLinks Links { get; set; }
    
    [JsonIgnore]
    public string? NextPageLink => Links?.Next;

    public override string ToString()
    {
        return string.Join("\n", Data?.Select((item, index) => $"[{index}] {item}") ?? Array.Empty<string>()) + "\n";
    }
    
    public class JobData
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("company_name")]
        public string CompanyName { get; set; }

        [JsonPropertyName("url")]
        public string JobUrl { get; set; }

        [JsonPropertyName("remote")]
        public bool IsRemote { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }
        
        [JsonPropertyName("salary")]
        public string? Salary { get; set; }
        
        [JsonPropertyName("created_at")]
        [JsonConverter(typeof(UnixDateTimeConverter))]
        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return
                $"- Title: {Title}\n" +
                $"- Company: {CompanyName}\n" +
                $"- Remote: {IsRemote}\n" +
                $"- Location: {Location}\n" +
                $"- Salary: {Salary ?? "N/A"}\n" +
				$"- CreatedAt: {CreatedAt:yyyy-MM-dd}\n" +
                $"- URL: {JobUrl}";
        }
    }

    public class PageLinks
    {
        [JsonPropertyName("next")]
        public string? Next { get; set; }
    }
}