using System.Text.Json;
using System.Text.Json.Serialization;

namespace JobSearch.DataScraper.Utils;

public class UnixDateTimeConverter : JsonConverter<DateTime>
{
	public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.Number)
			throw new JsonException("Expected number for Unix timestamp.");

		long unixTime = reader.GetInt64();
		return DateTimeOffset.FromUnixTimeSeconds(unixTime).UtcDateTime;
	}

	public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
	{
		long unixTime = new DateTimeOffset(value.ToUniversalTime()).ToUnixTimeSeconds();
		writer.WriteNumberValue(unixTime);
	}
}