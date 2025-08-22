namespace JwtSample.Entities;


public class User
{
  public int Id { get; set; }
  public required string UserName { get; set; }
  public required string PasswordHash { get; set; }
}
