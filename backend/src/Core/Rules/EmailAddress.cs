using System.Text.RegularExpressions;

namespace Domain.Rules;

public static class EmailAddress
{
    public const int MaxLength = 254;

    private static readonly Regex Pattern = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);

    // Simpelt formattjek: tekst@domæne.endelse uden mellemrum.
    public static bool IsValid(string? value)
        => value is not null && value.Length <= MaxLength && Pattern.IsMatch(value);

    // E-mails gemmes med små bogstaver, så sammenligning altid ignorerer store og små bogstaver.
    public static string Normalize(string value)
        => value.Trim().ToLowerInvariant();
}