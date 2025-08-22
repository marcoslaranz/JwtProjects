using JwtSample.DTOs;
using JwtSample.Repositories;
using JwtSample.Models;

namespace JwtSample.Services;

public interface IUserService
{
	public Task<UserDTO> AddAsync(LoginRequestModel userRequest);
	public Task<bool> DeleteAsync(UserDTO user);
	public Task<bool> UpdateAsync(UserDTO user);
	public Task<List<UserDTO>> GetAllAsync();
	public Task<UserDTO?> GetByIdAsync(int id);
	public Task<UserDTO?> GetByUserNameAsync(string username);
}