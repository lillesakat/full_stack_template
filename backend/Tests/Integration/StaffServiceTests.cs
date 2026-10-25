using Application.Exceptions;
using Application.Services;
using Domain.Entities;

namespace Tests.Integration;

public class StaffServiceTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly StaffService _service;

    public StaffServiceTests()
    {
        _service = new StaffService(_database.Context);
    }

    public void Dispose() => _database.Dispose();

    // Ansættelsen fra AddEmploymentAsync(TestDataBase) løber fra 2026-01-01 til 2026-12-31.
    [Theory]
    [InlineData("2026-12-01", "2026-12-31", 1)] // perioden ligger inden for ansættelsen
    [InlineData("2026-12-31", "2027-01-31", 1)] // deler én dag
    [InlineData("2027-01-01", "2027-01-31", 0)] // starter dagen efter ansættelsen slutter
    [InlineData("2025-12-01", "2025-12-31", 0)] // slutter dagen før ansættelsen starter
    public async Task GetForPeriodAsync_ReturnsEmploymentsOverlappingPeriod(string startDate, string endDate, int expectedCount)
    {
        // Arrange
        await _database.AddEmploymentAsync();
        var period = new Period { Name = "Testperiode", StartDate = DateOnly.Parse(startDate), EndDate = DateOnly.Parse(endDate) };
        _database.Context.Periods.Add(period);
        await _database.Context.SaveChangesAsync();

        // Act
        var staff = await _service.GetForPeriodAsync(period.Id);

        // Assert
        Assert.Equal(expectedCount, staff.Count);
    }

    [Fact]
    public async Task GetForPeriodAsync_UnknownPeriod_ThrowsNotFoundException()
    {
        // Act
        var act = () => _service.GetForPeriodAsync(999);

        // Assert
        await Assert.ThrowsAsync<NotFoundException>(act);
    }
}