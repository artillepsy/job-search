using System.Text.Json;
using JobSearch.DataScraper.Utils;

namespace JobServer.DataScraper.UnitTests.Utils;

public class UnixDateTimeConverterTests
{
	private readonly JsonSerializerOptions _options;

	public UnixDateTimeConverterTests()
	{
		_options = new JsonSerializerOptions();
		_options.Converters.Add(new UnixDateTimeConverter());
	}

	[Fact]
	public void Read_ShouldConvertUnixSecondsToDateTimeUtc()
	{
		// 1704067200 is 2024-01-01 00:00:00 UTC
		long unixTimestamp = 1704067200;
		string json = unixTimestamp.ToString();
		var expectedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		var result = JsonSerializer.Deserialize<DateTime>(json, _options);

		Assert.Equal(expectedDate, result);
		Assert.Equal(DateTimeKind.Utc, result.Kind);
	}

	[Fact]
	public void Write_ShouldConvertDateTimeToUnixSeconds()
	{
		var date = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		long expectedTimestamp = 1704067200;

		var json = JsonSerializer.Serialize(date, _options);

		Assert.Equal(expectedTimestamp.ToString(), json);
	}

	[Fact]
	public void Read_ShouldThrowJsonException_WhenTokenIsNotNumber()
	{
		string json = "\"2024-01-01\""; // A string instead of a number

		var exception = Assert.Throws<JsonException>(() => 
			JsonSerializer.Deserialize<DateTime>(json, _options));
        
		Assert.Equal("Expected number for Unix timestamp.", exception.Message);
	}
}