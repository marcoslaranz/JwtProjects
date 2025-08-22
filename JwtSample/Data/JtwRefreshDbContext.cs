using Microsoft.EntityFrameworkCore;
using JwtSample.Entities;

namespace JwtSample.Data;

public class JwtSampleDbContext(DbContextOptions<JwtSampleDbContext> options):DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
	
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
		// This enforce Token as primary key
		modelBuilder.Entity<RefreshToken>()
			          .HasKey(rt => rt.Token );
		
		modelBuilder.Entity<User>().HasData(
		   new {
			   Id = 1,
			   UserName = "admin",
			   PasswordHash = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
		});
    }
}