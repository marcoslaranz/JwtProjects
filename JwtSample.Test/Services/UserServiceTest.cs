//dotnet add package xunit;
//dotnet add package Moq
//dotnet add package Microsoft.NET.Test.Sdk

using Xunit;
using Moq;


using JwtSample.DTOs;
using JwtSample.Services;
using JwtSample.Models;
using JwtSample.Repositories;
using JwtSample.Data;
using JwtSample.Entities;
using JwtSample.Mappings;

namespace JwtSample.Test.Services;

public class UserServiceTest
{
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly UserService _userService;

    public UserServiceTest()
    {
        _userRepoMock = new Mock<IUserRepository>();
        _userService = new UserService(_userRepoMock.Object);
    }

    [Fact]
    public async Task AddAsync_ShouldReturnUserDTO_WhenUserIsAddedSuccessfully()
    {
        // Arrange
        var loginRequest = new LoginRequestModel
        {
            Username = "Antonio",
            Password = "UmDiaDeCao"
        };

        var userEntity = new User
        {
            Id = 1,
            UserName = "Antonio",
            PasswordHash = "hashed-password" // Assume hashing happens inside service
        };

        var expectedDto = new UserDTO
        {
            Id = 1,
            UserName = "Antonio",
            PasswordHash = "hashed-password" // Assume hashing happens inside service
        };

        // Tell to the Mock how behaver when call the
        // Method AddAsync.
        _userRepoMock.Setup(repo => repo.AddAsync(It.IsAny<User>()))
                     .ReturnsAsync(userEntity);

        // Act
        var result = await _userService.AddAsync(loginRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.UserName, result.UserName);
    }


    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenRepositoryUpdateSucceeds()
    {
        // Arrange
        var userDto = new UserDTO
        {
            Id = 1,
            UserName = "Antonio",
            PasswordHash = "hashed-password"
        };

        var userEntity = userDto.ToEntity(); // Manual mapping

        _userRepoMock.Setup(repo => repo.UpdateAsync(
                                              It.Is<User>
                                               (
                                                  u => u.Id == userDto.Id &&
                                                  u.UserName == userDto.UserName &&
                                                  u.PasswordHash == userDto.PasswordHash
                                                )
                                            )
                                        )
                                        .ReturnsAsync(true);

        // Act
        var result = await _userService.UpdateAsync(userDto);

        // Assert
        Assert.True(result);
        _userRepoMock.Verify(repo => repo.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    // Test this method: public Task<bool> DeleteAsync(User user);    
    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenRepositoryDeleteSucceeds()
    {
        // Arrange
        var user = new User { Id = 1, UserName = "Test User", PasswordHash = "hashed-password" };

        var mockRepo = new Mock<IUserRepository>();

        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<User>())).ReturnsAsync(true);



        var service = new UserService(mockRepo.Object);

        // Act
        var result = await service.DeleteAsync(user.ToUserDTO());

        // Assert
        Assert.True(result);
    }

    
   
    
    

    [Fact]
    public async Task GetByIdAsync_ShouldReturnUserDTO_WhenUserExists()
    {
        // Arrange
        var userId = 1;
        var userEntity = new User
        {
            Id = userId,
            UserName = "Antonio",
            PasswordHash = "hashed-password"
        };
        var expectedDto = userEntity.ToUserDTO();

        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(userEntity);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.UserName, result.UserName);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = 99;
        _userRepoMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((User)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnListOfUserDTOs_WhenUsersExist()
    {
        // Arrange
        var userEntities = new List<User>
        {
            new User { Id = 1, UserName = "Antonio", PasswordHash = "hashed-password-1" },
            new User { Id = 2, UserName = "Maria", PasswordHash = "hashed-password-2" }
        };
        var expectedDtos = userEntities.Select(u => u.ToUserDTO()).ToList();

        _userRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(userEntities);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedDtos.Count(), result.Count());
        Assert.Equal(expectedDtos[0].Id, result[0].Id);
        Assert.Equal(expectedDtos[0].UserName, result[0].UserName);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoUsersExist()    {
        // Arrange
        _userRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<User>());

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
    
                           
        

}