namespace JwtSample.Handlers;

public static class PasswordHashHandler
{
	public static bool VerifyPassword(string password, string hashpassword)
	{
		return BCrypt.Net.BCrypt.Verify(password, hashpassword);
	}
	
	public static string getHash(string password)
	{
		return BCrypt.Net.BCrypt.HashPassword(password);
	}
}
