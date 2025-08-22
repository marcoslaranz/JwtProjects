using JwtSample.Services;
using JwtSample.Models;
using JwtSample.Entities;

namespace JwtSample.Services;

public interface IJwtService
{
  public string getIssuer();
  public string getAudience();
  public string getKey();
  public int getTokenValidityMins();
  public int getRefreshTokenValidityMins();
  //public string GenerateToken(string username);
  public Task<LoginResponseModel?> Authenticate(LoginRequestModel request);
  public Task<LoginResponseModel?> GenerateJwtToken(User user);
  public Task<LoginResponseModel?> ValidateRefreshToken(string token);
}