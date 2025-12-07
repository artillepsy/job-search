namespace JobSearch.DataScraper.Extensions;

public static class ConfigurationManagerExtensions
{
	//todo: extract all the json configs from configuration folder
	public static void AddConfigJsonFiles(this ConfigurationManager configuration)
	{
		var appRootPath = Directory.GetCurrentDirectory();
		var configsRootPath = Path.Combine(appRootPath, "Configuration");
		
		if (!Directory.Exists(configsRootPath))
		{
			throw new DirectoryNotFoundException($"{configsRootPath} not found");
		}

		try
		{
			var jsonFilePaths = Directory.GetFiles(configsRootPath, "*.json", SearchOption.AllDirectories);
			Console.WriteLine($"Found {jsonFilePaths.Length} files:\n\n" + string.Join("\n", jsonFilePaths));

			foreach (var filePath in jsonFilePaths)
			{
				var relativePath = Path.GetRelativePath(appRootPath, filePath);
				configuration.AddJsonFile(path: relativePath, optional: false, reloadOnChange: true);
			}
		}
		catch (Exception e)
		{
			throw;
		}
	}
}