using JwtSample.Entities;
using JwtSample.DTOs;
using JwtSample.Handlers;

namespace JwtSample.Mappings;

public static class UserMappingUserMapping
{
	public static User ToEntity(this UserDTO user)
	{
		return new User()
		{
			UserName = user.UserName,
			PasswordHash = user.PasswordHash
		};
	}
	
	public static UserDTO ToUserDTO(this User user)
	{
		return new UserDTO
		{
			Id = user.Id,
			UserName = user.UserName,
			PasswordHash = user.PasswordHash
		};
	}
	
}