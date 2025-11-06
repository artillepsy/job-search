using System.Security.Cryptography;
using System.Text;

namespace JobSearch.Server.Services;

public class DevTokenService : ITokenService
{
	private readonly string _secret = "dev-secret";
	
	public string GenerateDevToken(string username)
	{
		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
		var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(username));
		return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+','-').Replace('/','_');
	}
}