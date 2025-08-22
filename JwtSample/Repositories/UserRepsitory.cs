using Microsoft.EntityFrameworkCore;
using JwtSample.Data;
using JwtSample.Entities;

namespace JwtSample.Repositories;


public class UserRepository: IUserRepository
{
	private readonly JwtSampleDbContext _dbContext;
	
	public UserRepository(JwtSampleDbContext dbContext)
	{
	   _dbContext = dbContext;
	}
	
	/*
	public Task<User?> GetByUserNameAsync(string username)
    {	
		return await _dbContext.User.FindFirstOrDefault( g => g.UserName = username);
	}*/
	
	
	
	public async Task<User?> GetByUserNameAsync(string username)
	{
		return await _dbContext.Users.FirstOrDefaultAsync(u => u.UserName == username);
	}
	
	public async Task<User> AddAsync(User user)
	{
		await _dbContext.AddAsync(user);
		
		await _dbContext.SaveChangesAsync();
		
		return user;
	}
	
	public async Task<bool> DeleteAsync(User user)
	{
		await _dbContext.Users
                     .Where(u => u.Id == user.Id )
                     .ExecuteDeleteAsync();
		
		await _dbContext.SaveChangesAsync();
		
		return true;
	}
	
	public async Task<bool> UpdateAsync(User user)
	{
		var UserToUpdate = _dbContext.Users.Find(user.Id);
		if( UserToUpdate is not null )
		{
			UserToUpdate.UserName = user.UserName;
			UserToUpdate.PasswordHash = user.PasswordHash;
		}
		
        await _dbContext.SaveChangesAsync();

		return true;
	}
	
	public async Task<List<User>> GetAllAsync()
	{
		return await _dbContext.Users.ToListAsync();
	}
	
	public async Task<User?> GetByIdAsync(int id)
	{
		return await _dbContext.Users.FindAsync(id);
	}
}