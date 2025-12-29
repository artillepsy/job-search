namespace JobSearch.Server.Services;

public interface ITokenService
{
	string GenerateDevToken(string username);
}