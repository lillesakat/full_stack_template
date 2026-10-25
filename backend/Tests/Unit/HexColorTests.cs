using Domain.Rules;

namespace Tests.Unit;

public class HexColorTests
{
    [Theory]
    [InlineData("#FF0000", true)]
    [InlineData("#00ff00", true)]
    [InlineData("FF0000", false)]
    [InlineData("#FFF", false)]
    [InlineData("#GG0000", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValid_ReturnsExpectedResult(string? value, bool expected)
    {
        // Act
        var result = HexColor.IsValid(value);

        // Assert
        Assert.Equal(expected, result);
    }
}