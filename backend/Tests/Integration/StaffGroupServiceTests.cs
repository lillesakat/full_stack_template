using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Application.Validators;

namespace Tests.Integration;

public class StaffGroupServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly StaffGroupService _service;

    public StaffGroupServiceTests()
    {
        _service = new StaffGroupService(_database.Context, new StaffGroupValidator());
    }

    public void Dispose() => _database.Dispose();

    [Fact]
    public async Task DeleteAsync_StaffGroupWithEmployments_ThrowsConflictException()
    {
        // Arrange
        var employment = await _database.AddEmploymentAsync();

        // Act
        var act = () => _service.DeleteAsync(employment.StaffGroupId);

        // Assert
        await Assert.ThrowsAsync<ConflictException>(act);
    }

    [Fact]
    public async Task DeleteAsync_StaffGroupWithoutEmployments_DeletesStaffGroup()
    {
        // Arrange
        var created = await _service.CreateAsync(new SaveStaffGroupRequest("Overlæge"));

        // Act
        await _service.DeleteAsync(created.Id);

        // Assert
        Assert.Empty(await _service.GetAllAsync());
    }

    [Fact]
    public async Task GetAllAsync_StaffGroupWithEmployment_ReturnsEmploymentCount()
    {
        // Arrange
        await _database.AddEmploymentAsync();

        // Act
        var staffGroups = await _service.GetAllAsync();

        // Assert
        Assert.Equal(1, staffGroups.Single().EmploymentCount);
    }
}