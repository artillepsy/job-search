using System.Security.Cryptography;
using System.Text;

namespace JobSearch.Api.Services;

public class DevTokenService : ITokenService
{
	private readonly string _secret = "dev-secret";
	
	// todo: make sure that the token is generated randomly (based on datetime)? and is unique. Make it able to expire
	// todo: or check frameworks for auth (OAuth)
	public string GenerateDevToken(string username)
	{
		using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
		var bytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(username));
		return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+','-').Replace('/','_');
	}
}