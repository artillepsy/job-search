using JobSearch.DataScraper.Extensions;
using JobSearch.DataScraper.Scraping.Scrapers;
using JobServer.DataScraper.UnitTests.Scraping.Scrapers.Mocks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobServer.DataScraper.UnitTests.Extensions;

public class ServiceCollectionExtensionsTests
{
	[Fact]
	public void AddScrapers_ShouldRegisterScrapersWithCorrectConfigSection()
	{
		var services = new ServiceCollection();
   
		services.AddSingleton(new Mock<ILogger<ScraperBase>>().Object);
		services.AddSingleton(new Mock<IHttpClientFactory>().Object);
		services.AddSingleton(new Mock<IServiceScopeFactory>().Object);
		
		var configValues = new Dictionary<string, string?>
		{
			{ "Scrapers:MockSection:BaseUrl", "https://test.api.com/" },
			{ "Scrapers:MockSection:HttpClientName", "TestClient" }
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(configValues)
			.Build();

		services.AddScrapers(configuration, typeof(MockScraper).Assembly);
		
		var isRegistered = services.Any(x => x.ServiceType == typeof(MockScraper));
		Assert.True(isRegistered, "MockScraper was not found in the ServiceCollection");
		
		var serviceProvider = services.BuildServiceProvider();
		var scraper = serviceProvider.GetService<MockScraper>();
		
		Assert.NotNull(scraper);
		Assert.True(scraper is MockScraper);
	}
	
	[Fact]
	public void AddScrapers_ShouldIgnoreClasses_WithoutMetadataAttribute()
	{
		var services = new ServiceCollection();
		var configuration = new ConfigurationBuilder().Build();

		services.AddScrapers(configuration, typeof(NoMetadataAttributeMockScraper).Assembly);
    
		Assert.DoesNotContain(services, d => d.ServiceType == typeof(NoMetadataAttributeMockScraper));
	}
}