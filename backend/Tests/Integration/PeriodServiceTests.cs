using Application.DTOs;
using Application.Exceptions;
using Application.Services;
using Application.Validators;

namespace Tests.Integration;

public class PeriodServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly PeriodService _service;

    public PeriodServiceTests()
    {
        _service = new PeriodService(_database.Context, new PeriodValidator(_database.Context));
    }

    public void Dispose() => _database.Dispose();

    private static SavePeriodRequest December2026()
        => new("December 2026", new DateOnly(2026, 12, 1), new DateOnly(2026, 12, 31));

    private static SavePeriodRequest January2027()
        => new("Januar 2027", new DateOnly(2027, 1, 1), new DateOnly(2027, 1, 31));

    [Fact]
    public async Task CreateAsync_ValidPeriod_SavesPeriod()
    {
        // Arrange
        var request = December2026();

        // Act
        await _service.CreateAsync(request);

        // Assert
        var periods = await _service.GetAllAsync();
        Assert.Single(periods);
        Assert.Equal("December 2026", periods[0].Name);
    }

    [Fact]
    public async Task CreateAsync_PeriodDirectlyAfterExisting_IsAllowed()
    {
        // Arrange
        await _service.CreateAsync(December2026());

        // Act
        await _service.CreateAsync(January2027());

        // Assert
        var periods = await _service.GetAllAsync();
        Assert.Equal(2, periods.Count);
    }

    [Fact]
    public async Task CreateAsync_OverlappingPeriod_ThrowsValidationException()
    {
        // Arrange
        await _service.CreateAsync(December2026());
        var overlapping = new SavePeriodRequest("Overlap", new DateOnly(2026, 12, 15), new DateOnly(2027, 1, 15));

        // Act
        var act = () => _service.CreateAsync(overlapping);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("startDate"));
    }

    [Fact]
    public async Task CreateAsync_EndDateBeforeStartDate_ThrowsValidationException()
    {
        // Arrange
        var request = new SavePeriodRequest("Forkert", new DateOnly(2026, 12, 31), new DateOnly(2026, 12, 1));

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("endDate"));
    }

    [Fact]
    public async Task UpdateAsync_KeepingOwnDates_DoesNotOverlapWithItself()
    {
        // Arrange
        var created = await _service.CreateAsync(December2026());
        var renamed = December2026() with { Name = "December 2026 (rettet)" };

        // Act
        var updated = await _service.UpdateAsync(created.Id, renamed);

        // Assert
        Assert.Equal("December 2026 (rettet)", updated.Name);
    }

    [Fact]
    public async Task UpdateAsync_UnknownId_ThrowsNotFoundException()
    {
        // Act
        var act = () => _service.UpdateAsync(999, December2026());

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }

    [Fact]
    public async Task CreateAsync_MissingDates_ThrowsValidationExceptionForBothDates()
    {
        // Arrange
        var request = new SavePeriodRequest("Uden datoer", null, null);

        // Act
        var act = () => _service.CreateAsync(request);

        // Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(act);
        Assert.True(exception.Errors.ContainsKey("startDate"));
        Assert.True(exception.Errors.ContainsKey("endDate"));
    }
}