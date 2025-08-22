using Microsoft.EntityFrameworkCore;
using JwtSample.Repositories;
using JwtSample.Entities;
using JwtSample.Mappings;
using JwtSample.DTOs;
using JwtSample.Handlers;
using JwtSample.Models;

namespace JwtSample.Services;

public class UserService
{
	private readonly IUserRepository _repo;
	
	public UserService(IUserRepository repo)
	{
	   _repo = repo;
	}
	
	
	public async Task<UserDTO?> GetByUserNameAsync(string username)
	{
		var user = await _repo.GetByUserNameAsync(username);
		
		return user.ToUserDTO();
	}
	
	public async Task<UserDTO> AddAsync(LoginRequestModel userRequest)
	{
		UserDTO userDto = new UserDTO()
		{
			UserName = userRequest.Username,
			PasswordHash = PasswordHashHandler.getHash(userRequest.Password)
		};
		
		var user = await _repo.AddAsync(userDto.ToEntity());
		return user.ToUserDTO();
	}
	
	
	
	
	
	
	public async Task<bool> DeleteAsync(UserDTO user)
	{
		return await _repo.DeleteAsync(user.ToEntity());
	}
	
	public async Task<bool> UpdateAsync(UserDTO user)
	{
		await _repo.UpdateAsync(user.ToEntity());
		return true;
	}
	
	public async Task<List<UserDTO>> GetAllAsync()
	{
		var users = await _repo.GetAllAsync(); // ✅ This returns List<User>
		if( users is not null )
			return users!.Select(u => u!.ToUserDTO()).ToList(); // ✅ Now you can use Select
		
		return new List<UserDTO>(); // Return empty list instead of null
	}
	
	public async Task<UserDTO?> GetByIdAsync(int id)
	{
		var user =  await _repo.GetByIdAsync(id);
		if(user is not null)
			return user.ToUserDTO();
		
		return null;
	}
}