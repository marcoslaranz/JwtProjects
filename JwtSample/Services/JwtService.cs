using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using JwtSample.Models;
using JwtSample.Entities;
using JwtSample.Repositories;
using JwtSample.Handlers;

namespace JwtSample.Services;

public class JwtService: IJwtService
{
  private readonly JwtConfig _config;
  private readonly IUserRepository _userRepository;
  private readonly RefreshTokenRepository _refreshTokenRepository;
  private readonly ILogger<JwtService> _logger;
  
  public JwtService(JwtConfig config, IUserRepository userRepository, RefreshTokenRepository rtoken, ILogger<JwtService> logger)
  {
	  _config = config;
	  _userRepository = userRepository;
	  _refreshTokenRepository = rtoken;
	  _logger = logger;
  }
  
  public string getIssuer() => _config.Issuer;
    
  public string getAudience() => _config.Audience;
    
  public string getKey() => _config.Key;
  
  public int getTokenValidityMins() => _config.TokenValidityMins;
  
  public int getRefreshTokenValidityMins() => _config.RefreshTokenValidityMins;
	
  public async Task<LoginResponseModel?> Authenticate(LoginRequestModel request)
  {
	_logger.LogInformation($"Authenticate: Fetching LoginRequestModel username: {request.Username}");

	
	if(string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
	{
		return null;
	}
	
	var user = await _userRepository.GetByUserNameAsync(request.Username);
	if(user is null || !PasswordHashHandler.VerifyPassword(request.Password, user.PasswordHash!))
	{
		return null;
	}
	
	return await GenerateJwtToken(user);
  }	
	
/*	
  public string GenerateToken(string username)
  {
  	var claim = new[]
  	{
  		new Claim(JwtRegisteredClaimNames.Sub, username),
  		new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
  	};
	
  	var key= new SymmetricSecurityKey(Encoding.UTF8.GetBytes(getKey()));
  	var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
  	var token = new JwtSecurityToken(
  		   issuer: getIssuer(),
  		   audience: getAudience(),
  		   claims: claim,
  		   expires: DateTime.Now.AddHours(1),
  		   signingCredentials: cred);
	
  	return new JwtSecurityTokenHandler().WriteToken(token);
  }
  */
  
  public async Task<LoginResponseModel?> GenerateJwtToken(User user)
  {
	_logger.LogInformation($"GenerateJwtToken: Fetching LoginRequestModel username: {user.UserName}");
	
	var tokenExpiryTimeStamp = DateTime.UtcNow.AddMinutes(getTokenValidityMins());
	var key = Encoding.UTF8.GetBytes(getKey());
	
	var token = new JwtSecurityToken(
			issuer: getIssuer(),
			audience: getAudience(),
			[
				 new Claim(ClaimTypes.Name, user.UserName!),
                 new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())  // Also recommended
				 // new Claim(ClaimTypes.Role, "Admin") // Or dynamically assign based on user
				 // if do that you can use this role name "Admin" 
				 // in the Authorization of an specific EndPoint that you
				 // want only "Admin" to access.
			],
			expires: tokenExpiryTimeStamp,
			//signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key),
			//							  SecurityAlgorithms.HmacSha512Signature)
			signingCredentials: new SigningCredentials(
                                       new SymmetricSecurityKey(key),
                                       SecurityAlgorithms.HmacSha512 ) // Use this instead of HmacSha512Signature. Compatible with jwt.io website debuger
			
		);
			
	var accessToken = new JwtSecurityTokenHandler().WriteToken(token);	
	
	return new LoginResponseModel
	{
		Username = user.UserName,
		AccessToken = accessToken,
		ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.UtcNow).TotalSeconds,
		RefreshToken = await GenerateRefreshToken(user.Id)
	};
  }
	
	
  private async Task<string> GenerateRefreshToken(int userId)
  {
	_logger.LogInformation($"GenerateRefreshToken: userId: {userId}");
	
  	var refreshToken = new RefreshToken
  	{
  		Token = Guid.NewGuid().ToString(),
  		Expiry = DateTime.UtcNow.AddMinutes(getRefreshTokenValidityMins()),
  		UserId = userId
  	};
  	
  	await _refreshTokenRepository.CreateAsync(refreshToken);
  	
  	return refreshToken.Token;
  }
	
	
  public async Task<LoginResponseModel?> ValidateRefreshToken(string token)
  {
	_logger.LogInformation($"ValidateRefreshToken: token: {token}");
	
	if (string.IsNullOrWhiteSpace(token))
    {
        _logger.LogWarning("ValidateRefreshToken called with null or empty token.");
        return null;
    }
	
  	var refreshToken = await _refreshTokenRepository.Get(token);
	
  	if(refreshToken is null || refreshToken.Expiry < DateTime.UtcNow)
  	{
		_logger.LogInformation("1. ValidateRefreshToken: Now={Now}", DateTime.UtcNow );
		
  		return null;
  	}
	
	_logger.LogInformation("2. ValidateRefreshToken: Expiry={Expiry}, Now={Now}, Expired={Expired}",
				refreshToken.Expiry,
				DateTime.UtcNow,
				refreshToken.Expiry < DateTime.UtcNow
			);
	
	
  	
  	await _refreshTokenRepository.Delete(refreshToken);
  	
  	var user = await _userRepository.GetByIdAsync(refreshToken.UserId);
  	if(user is null) 
  	{
  		return null;
  	}
  	return await GenerateJwtToken(user);
  }
}