using System.Text.Json;

namespace JobSearch.DataScraper.Services.Utils;

public static class ConfigLoader
{
	public static T Load<T>(string configsPath = "Src/Configs") where T : class
	{
		var filename = $"{typeof(T).Name}.json";
		var filePath = Path.Combine(configsPath, filename);

		var json = File.ReadAllText(filePath);
		return JsonSerializer.Deserialize<T>(json) ?? throw new InvalidOperationException();
	}
}