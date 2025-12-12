using System.Text;
using System.Text.Json;

namespace JobSearch.DataScraper.Utils;

public static class MemoryHelper
{
	public static long GetSerializedSize<T>(T obj)
	{
		var json = JsonSerializer.Serialize(obj);
		return Encoding.UTF8.GetByteCount(json);
	}
}