using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Application.Validators;
using Domain.Entities;

namespace Tests.Integration;

public class EmploymentServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly EmploymentService _service;

    public EmploymentServiceTests()
    {
        _service = new EmploymentService(_database.Context, new EmploymentValidator(_database.Context));
    }

    public void Dispose() => _database.Dispose();

    private static SaveEmploymentRequest RequestLike(Employment existing, DateOnly startDate, DateOnly endDate)
        => new(existing.UserId, existing.DepartmentId, existing.StaffGroupId, 37, startDate, endDate);

    [Fact]
    public async Task CreateAsync_OverlappingEmploymentForSamePerson_ThrowsValidationException()
    {
        // Arrange
        var existing = await _database.AddEmploymentAsync();
        var request = RequestLike(existing, new DateOnly(2026, 6, 1), new DateOnly(2027, 5, 31));

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("startDate"));
    }

    [Fact]
    public async Task CreateAsync_EmploymentDirectlyAfterExisting_IsAllowed()
    {
        // Arrange
        var existing = await _database.AddEmploymentAsync();
        var request = RequestLike(existing, new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31));

        // Act
        var created = await _service.CreateAsync(request);

        // Assert
        Assert.Equal(existing.UserId, created.UserId);
    }

    [Fact]
    public async Task CreateAsync_OverlappingEmploymentForDifferentPerson_IsAllowed()
    {
        // Arrange
        var existing = await _database.AddEmploymentAsync();
        var otherPerson = new User { Name = "Bo Nielsen", Email = "bo@hospital.dk", IsStaff = true };
        _database.Context.Users.Add(otherPerson);
        await _database.Context.SaveChangesAsync();

        var request = RequestLike(existing, existing.StartDate, existing.EndDate) with { UserId = otherPerson.Id };

        // Act
        var created = await _service.CreateAsync(request);

        // Assert
        Assert.Equal("Bo Nielsen", created.UserName);
    }

    [Fact]
    public async Task CreateAsync_PersonWithoutStaffRole_ThrowsValidationException()
    {
        // Arrange
        var existing = await _database.AddEmploymentAsync();
        var adminOnly = new User { Name = "Karen Admin", Email = "karen@hospital.dk", IsAdmin = true, IsStaff = false };
        _database.Context.Users.Add(adminOnly);
        await _database.Context.SaveChangesAsync();

        var request = RequestLike(existing, new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31)) with { UserId = adminOnly.Id };

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("userId"));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(38)]
    public async Task CreateAsync_HoursOutsideRange_ThrowsValidationException(int hours)
    {
        // Arrange
        var existing = await _database.AddEmploymentAsync();
        var request = RequestLike(existing, new DateOnly(2027, 1, 1), new DateOnly(2027, 12, 31)) with { HoursPerWeek = hours };

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("hoursPerWeek"));
    }

    [Fact]
    public async Task UpdateAsync_KeepingOwnDates_DoesNotOverlapWithItself()
    {
        // Arrange
        var existing = await _database.AddEmploymentAsync();
        var request = RequestLike(existing, existing.StartDate, existing.EndDate) with { HoursPerWeek = 30 };

        // Act
        var updated = await _service.UpdateAsync(existing.Id, request);

        // Assert
        Assert.Equal(30, updated.HoursPerWeek);
    }
}