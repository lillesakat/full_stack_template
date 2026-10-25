using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Application.Validators;

namespace Tests.Integration;

public class DepartmentServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly DepartmentService _service;

    public DepartmentServiceTests()
    {
        _service = new DepartmentService(_database.Context, new DepartmentValidator());
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task CreateAsync_LowercaseColor_SavesColorInUppercase()
    {
        // Arrange
        var request = new SaveDepartmentRequest("Skadestue", "#ff0000");

        // Act
        var created = await _service.CreateAsync(request);

        // Assert
        Assert.Equal("#FF0000", created.Color);
    }

    [Fact]
    public async Task CreateAsync_InvalidColor_ThrowsValidationException()
    {
        // Arrange
        var request = new SaveDepartmentRequest("Skadestue", "rød");

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("color"));
    }

    [Fact]
    public async Task DeleteAsync_DepartmentWithEmployments_ThrowsConflictException()
    {
        // Arrange
        var employment = await _database.AddEmploymentAsync();

        // Act
        var act = () => _service.DeleteAsync(employment.DepartmentId);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task DeleteAsync_DepartmentWithoutEmployments_DeletesDepartment()
    {
        // Arrange
        var created = await _service.CreateAsync(new SaveDepartmentRequest("Sengeafsnit A", "#00FF00"));

        // Act
        await _service.DeleteAsync(created.Id);

        // Assert
        Assert.Empty(await _service.GetAllAsync());
    }
}