using System.Text.RegularExpressions;

namespace Domain.Rules;

public static class HexColor
{
    private static readonly Regex Pattern = new("^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

    // En gyldig HEX-farve er # efterfulgt af præcis seks hexadecimale tegn, fx #FF0000.
    public static bool IsValid(string? value)
        => value is not null && Pattern.IsMatch(value);
}