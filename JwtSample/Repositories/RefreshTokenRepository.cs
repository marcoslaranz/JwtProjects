using JwtSample.Entities;
using JwtSample.Data;


namespace JwtSample.Repositories;

public class RefreshTokenRepository
{
	private readonly JwtSampleDbContext _dbContext;
	
	public RefreshTokenRepository(JwtSampleDbContext dbContext)
	{
		_dbContext = dbContext;
	}

	public async Task<int> BulkDelete(DateTime dt)
	{
		var expiredTokens = _dbContext.RefreshTokens
			.Where(token => token.Expiry < dt);

		_dbContext.RefreshTokens.RemoveRange(expiredTokens);
		return await _dbContext.SaveChangesAsync();
	}

	
	public async Task CreateAsync(RefreshToken refreshToken)
	{
		Console.WriteLine($"Saving RefreshToken: {refreshToken.Id}-{refreshToken.UserId}-{refreshToken.Token}-{refreshToken.Expiry}");
		
		await _dbContext.RefreshTokens.AddAsync(refreshToken);
		
		await _dbContext.SaveChangesAsync();
	}
	
	
	
	
	public async Task<RefreshToken?> Get(string token)
	{
		if(!string.IsNullOrEmpty(token))
			return await _dbContext.RefreshTokens.FindAsync(token);
		return null;
	}
	
	
	
	
	
	public async Task Delete(RefreshToken refreshToken)
	{
		if(refreshToken is not null)
		{
			_dbContext.RefreshTokens.Remove(refreshToken);
            await _dbContext.SaveChangesAsync();
		}
		return;	
	}
}