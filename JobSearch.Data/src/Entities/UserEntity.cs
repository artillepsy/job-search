namespace JobSearch.Data.Entities;

public class UserEntity
{
	public int Id { get; set; }
	public string Username { get; set; } = "";
	public string PasswordHash { get; set; } = "";
}