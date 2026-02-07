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
	private static void SetupMockDependencies(ServiceCollection services)
	{
		services.AddSingleton(new Mock<ILogger<ScraperBase>>().Object);
		services.AddSingleton(new Mock<IHttpClientFactory>().Object);
		services.AddSingleton(new Mock<IServiceScopeFactory>().Object);
	}

	[Fact]
	public void AddScrapers_ShouldRegisterScrapersWithCorrectConfigSection()
	{
		var services = new ServiceCollection();
   
		SetupMockDependencies(services);

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
		var configValues = new Dictionary<string, string?>
		{
			{ "Scrapers:MockSection:BaseUrl", "https://test.api.com/" },
			{ "Scrapers:MockSection:HttpClientName", "TestClient" }
		};
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(configValues)
			.Build();

		services.AddScrapers(configuration, typeof(NoMetadataAttributeMockScraper).Assembly);
    
		Assert.DoesNotContain(services, d => d.ServiceType == typeof(NoMetadataAttributeMockScraper));
	}
	
	[Fact]
	public void AddScrapers_ShouldThrowException_WhenSectionIsMissing()
	{
		var services = new ServiceCollection();
		SetupMockDependencies(services);
		
		var configuration = new ConfigurationBuilder()
			.AddInMemoryCollection(new Dictionary<string, string?>()) 
			.Build();

		Assert.Throws<InvalidOperationException>(() => 
			services.AddScrapers(configuration, typeof(MockScraper).Assembly));
	}
}