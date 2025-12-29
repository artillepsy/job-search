namespace JobSearch.Api.Services;

public interface ITokenService
{
	string GenerateDevToken(string username);
}