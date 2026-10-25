using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Application.Validators;

namespace Tests.Integration;

public class UserServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _service = new UserService(_database.Context, new UserValidator(_database.Context));
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task CreateAsync_EmailAlreadyUsedWithDifferentCase_ThrowsValidationException()
    {
        // Arrange
        await _service.CreateAsync(new SaveUserRequest("Anna Hansen", "anna@hospital.dk", false, true));
        var duplicate = new SaveUserRequest("Anna H.", "ANNA@hospital.dk", true, false);

        // Act
        var act = () => _service.CreateAsync(duplicate);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("email"));
    }

    [Fact]
    public async Task UpdateAsync_KeepingOwnEmail_IsAllowed()
    {
        // Arrange
        var created = await _service.CreateAsync(new SaveUserRequest("Anna Hansen", "anna@hospital.dk", false, true));
        var renamed = new SaveUserRequest("Anna Jensen", "anna@hospital.dk", false, true);

        // Act
        var updated = await _service.UpdateAsync(created.Id, renamed);

        // Assert
        Assert.Equal("Anna Jensen", updated.Name);
    }

    [Fact]
    public async Task CreateAsync_NoRoles_ThrowsValidationException()
    {
        // Arrange
        var request = new SaveUserRequest("Anna Hansen", "anna@hospital.dk", false, false);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("roles"));
    }

    [Fact]
    public async Task UpdateAsync_RemovingStaffRoleFromUserWithEmployments_ThrowsValidationException()
    {
        // Arrange
        var employment = await _database.AddEmploymentAsync();
        var request = new SaveUserRequest(employment.User.Name, employment.User.Email, true, false);

        // Act
        var act = () => _service.UpdateAsync(employment.UserId, request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("isStaff"));
    }

    [Fact]
    public async Task DeleteAsync_UserWithEmployments_ThrowsConflictException()
    {
        // Arrange
        var employment = await _database.AddEmploymentAsync();

        // Act
        var act = () => _service.DeleteAsync(employment.UserId);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }
}