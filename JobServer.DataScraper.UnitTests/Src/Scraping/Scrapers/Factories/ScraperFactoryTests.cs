using JobSearch.DataScraper.Scraping.Scrapers;
using JobSearch.DataScraper.Scraping.Scrapers.Factories;
using JobServer.DataScraper.UnitTests.Scraping.Scrapers.Mocks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace JobServer.DataScraper.UnitTests.Scraping.Scrapers.Factories;

public class ScraperFactoryTests
{
	private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<IServiceScope> _serviceScopeMock;
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly ILogger<ScraperFactory> _logger;

    public ScraperFactoryTests()
    {
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _serviceScopeMock = new Mock<IServiceScope>();
        _serviceProviderMock = new Mock<IServiceProvider>();
        _logger = loggerFactory.CreateLogger<ScraperFactory>();

        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(_serviceScopeMock.Object);
        _serviceScopeMock.Setup(x => x.ServiceProvider).Returns(_serviceProviderMock.Object);
    }
    
    [Theory]
    [InlineData("mocksection")]
    [InlineData("MOCKSECTION")]
    [InlineData("mOcKsEcTiOn")]
    public void CreateScraper_ShouldReturnScraper_RegardlessOfCasing(string inputName)
    {
        var scraperLoggerMock = new Mock<ILogger<ScraperBase>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var testAssembly = typeof(MockScraper).Assembly;
    
        var expectedScraper = new MockScraper(
            scraperLoggerMock.Object, 
            httpClientFactoryMock.Object, 
            _scopeFactoryMock.Object
        );
    
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(MockScraper)))
            .Returns(expectedScraper);

        var factory = new ScraperFactory(
            _scopeFactoryMock.Object, 
            _logger, 
            testAssembly
        );
    
        var result = factory.CreateScraper(inputName);

        Assert.NotNull(result);
        Assert.Same(expectedScraper, result);
        _scopeFactoryMock.Verify(x => x.CreateScope(), Times.Once);
    }

    [Fact]  
    public void CreateScraper_ShouldReturnCorrectScraper_WhenScraperExists()
    {
        var scraperName = "MockSection"; 
        var scraperLoggerMock = new Mock<ILogger<ScraperBase>>();
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var testAssembly = typeof(MockScraper).Assembly;
        
        var expectedScraper = new MockScraper(
            scraperLoggerMock.Object, 
            httpClientFactoryMock.Object, 
            _scopeFactoryMock.Object
        );
        
        _serviceProviderMock
            .Setup(x => x.GetService(typeof(MockScraper)))
            .Returns(expectedScraper);

        var factory = new ScraperFactory(
            _scopeFactoryMock.Object, 
            _logger, 
            testAssembly
        );
        
        var result = factory.CreateScraper(scraperName);

        Assert.NotNull(result);
        Assert.Same(expectedScraper, result);
        _scopeFactoryMock.Verify(x => x.CreateScope(), Times.Once);
    }

    [Fact]
    public void CreateScraper_ShouldThrowArgumentException_WhenScraperDoesNotExist()
    {
        var factory = new ScraperFactory(_scopeFactoryMock.Object, _logger);
        var invalidName = "NonExistentScraper";

        Assert.Throws<ArgumentException>(() => factory.CreateScraper(invalidName));
    }
}