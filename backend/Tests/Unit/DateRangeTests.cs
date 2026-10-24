using Domain.Rules;

namespace Tests.Unit;

public class DateRangeTests
{
    [Theory]
    [InlineData("2026-12-01", "2026-12-31", "2027-01-01", "2027-01-31", false)] // ligger op ad hinanden
    [InlineData("2026-12-01", "2026-12-31", "2026-12-31", "2027-01-31", true)]  // deler én dag
    [InlineData("2026-12-01", "2026-12-31", "2026-12-15", "2027-01-15", true)]  // delvist overlap
    [InlineData("2026-12-01", "2026-12-31", "2026-12-10", "2026-12-20", true)]  // B ligger inde i A
    [InlineData("2026-12-10", "2026-12-20", "2026-12-01", "2026-12-31", true)]  // A ligger inde i B
    [InlineData("2026-12-01", "2026-12-31", "2026-12-01", "2026-12-31", true)]  // identiske
    [InlineData("2027-01-01", "2027-01-31", "2026-12-01", "2026-12-31", false)] // B ligger før A
    public void Overlaps_ShouldReturnExpectedResult(string startA, string endA, string startB, string endB, bool expected)
    {
        // Arrange
        var aStart = DateOnly.Parse(startA);
        var aEnd = DateOnly.Parse(endA);
        var bStart = DateOnly.Parse(startB);
        var bEnd = DateOnly.Parse(endB);

        // Act
        var result = DateRange.Overlaps(aStart, aEnd, bStart, bEnd);

        // Assert
        Assert.Equal(expected, result);
    }

}
