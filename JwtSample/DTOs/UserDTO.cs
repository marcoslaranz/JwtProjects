namespace JwtSample.DTOs;

public class UserDTO
{
  public int Id { get; set; }
  public required string UserName { get; set; }
  public required string PasswordHash { get; set; }
}
