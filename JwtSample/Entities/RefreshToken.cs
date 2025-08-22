using System.ComponentModel.DataAnnotations;
namespace JwtSample.Entities;

public class RefreshToken
{
  public int Id { get; set; }
  public int UserId { get; set; }
  [Required] public string? Token { get; set; } = default;  // define it as Primary key
  // I could use this:
  // [key]
  // public string? Token { get; set; } = default;  // define it as Primary key
  // Then EF would threat it as primary key wouthout need
  // to change DbContext
  
  public DateTime Expiry { get; set; }
}