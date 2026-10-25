namespace Application.Validators;

// Fælles regel for navnefelter: påkrævet og højst 100 tegn.
public static class NameValidation
{
    public const int MaxLength = 100;

    public static void Validate(string? name, IDictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
            errors["name"] = "Navn er påkrævet.";
        else if (name.Trim().Length > MaxLength)
            errors["name"] = $"Navn må højst være {MaxLength} tegn.";
    }
}