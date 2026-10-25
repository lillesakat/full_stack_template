using Domain.Rules;

namespace Tests.Unit;

public class EmailAddressTests
{
    [Theory]
    [InlineData("anna@hospital.dk", true)]
    [InlineData("anna.hansen@region.hospital.dk", true)]
    [InlineData("anna@hospital", false)]
    [InlineData("anna hospital.dk", false)]
    [InlineData("anna@@hospital.dk", false)]
    [InlineData("@hospital.dk", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValid_ReturnsExpectedResult(string? value, bool expected)
    {
        // Act
        var result = EmailAddress.IsValid(value);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void Normalize_MixedCaseWithSpaces_ReturnsTrimmedLowercase()
    {
        // Act
        var result = EmailAddress.Normalize("  Anna@Hospital.DK ");

        // Assert
        Assert.Equal("anna@hospital.dk", result);
    }
}