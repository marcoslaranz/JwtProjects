using JwtSample.Entities;

namespace JwtSample.Repositories;

public interface IUserRepository
{
	public Task<User> AddAsync(User user);
	public Task<bool> DeleteAsync(User user);
	public Task<bool> UpdateAsync(User user);
	public Task<List<User>> GetAllAsync();
	public Task<User?> GetByIdAsync(int id);
	public Task<User?> GetByUserNameAsync(string username);
}