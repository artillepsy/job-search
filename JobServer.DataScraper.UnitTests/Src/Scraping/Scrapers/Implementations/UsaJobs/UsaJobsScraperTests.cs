using System.Net;
using JobSearch.DataScraper.Data.Repositories;
using JobSearch.DataScraper.Scraping.Scrapers.Implementations.UsaJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;

namespace JobServer.DataScraper.UnitTests.Scraping.Scrapers.Implementations.UsaJobs;

public class UsaJobsScraperTests
{
	private readonly Mock<IHttpClientFactory> _httpFactoryMock;
    private readonly Mock<IServiceScopeFactory> _scopeFactoryMock;
    private readonly Mock<ILogger<UsaJobsScraper>> _loggerMock;
    private readonly Mock<IJobRepository> _repositoryMock;

    public UsaJobsScraperTests()
    {
        _httpFactoryMock = new Mock<IHttpClientFactory>();
        _scopeFactoryMock = new Mock<IServiceScopeFactory>();
        _loggerMock = new Mock<ILogger<UsaJobsScraper>>();
        _repositoryMock = new Mock<IJobRepository>();

        var scopeMock = new Mock<IServiceScope>();
        var serviceProviderMock = new Mock<IServiceProvider>();
        
        _scopeFactoryMock.Setup(x => x.CreateScope()).Returns(scopeMock.Object);
        scopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IJobRepository)))
            .Returns(_repositoryMock.Object);
    }
    
    [Theory]
    [InlineData("Scrapers:UsaJobs")]
    [InlineData("scrapers:usajobs")]
    [InlineData("scrAperS:uSaJoBs")]
    public void Constructor_ShouldBindConfiguration_CaseInsensitively(string sectionName)
    {
        var testHttpClientName = "TestClient";
        var configValues = new Dictionary<string, string?>
        {
            { $"{sectionName}:BaseUrl", "https://test.api.com/" },
            { $"{sectionName}:Website", "test-website" },
            { $"{sectionName}:HttpClientName", testHttpClientName },
            { $"{sectionName}:PageStartIndex", "1" },
            { $"{sectionName}:SearchIntervalMin", "100" },
            { $"{sectionName}:SearchIntervalMax", "500" }
        };

        var rootConfig = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var section = rootConfig.GetSection($"{sectionName}");

        var scraper = new UsaJobsScraper(
            _loggerMock.Object, 
            _httpFactoryMock.Object, 
            section, 
            _scopeFactoryMock.Object);

        Assert.NotNull(scraper);
    }
    
    [Fact]
    public void Constructor_ShouldThrow_WhenSectionDoesNotExist()
    {
        var emptyConfig = new ConfigurationBuilder().Build(); // No keys added
        var missingSection = emptyConfig.GetSection("Scrapers:UsaJobs");

        Assert.Throws<ArgumentException>(() => 
            new UsaJobsScraper(_loggerMock.Object, _httpFactoryMock.Object, missingSection, _scopeFactoryMock.Object)
        );
    }
    
    [Fact]
    public void Constructor_ShouldThrow_WhenBaseUrlIsEmpty()
    {
        var configValues = new Dictionary<string, string?>
        {
            { "BaseUrl", "" },
            { "HttpClientName", "TestClient" }
        };
        var section = new ConfigurationBuilder().AddInMemoryCollection(configValues).Build();

        Assert.Throws<ArgumentException>(() => 
            new UsaJobsScraper(_loggerMock.Object, _httpFactoryMock.Object, section, _scopeFactoryMock.Object)
        );
    }

    [Fact]
    public async Task ScrapeAsync_ShouldUseBaseUrlFromConfig()
    {
        var configValues = new Dictionary<string, string?>
        {
            {"BaseUrl", "https://api.test.com/jobs?page="},
            {"HttpClientName", "TestClient"},
            {"Website", "TestWebsite"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        var handlerMock = new Mock<HttpMessageHandler>();
        var client = new HttpClient(handlerMock.Object);
        _httpFactoryMock.Setup(x => x.CreateClient("TestClient")).Returns(client);

        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage 
            { 
                StatusCode = HttpStatusCode.OK, 
                Content = new StringContent("{\"data\": [], \"links\": {\"next\": null}}") 
            });

        var scraper = new UsaJobsScraper(_loggerMock.Object, _httpFactoryMock.Object, configuration, _scopeFactoryMock.Object);

        await scraper.ScrapeAsync(CancellationToken.None);

        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Once(),
            ItExpr.Is<HttpRequestMessage>(req => req.RequestUri!.ToString().StartsWith("https://api.test.com/jobs?page=")),
            ItExpr.IsAny<CancellationToken>()
        );
    }
}