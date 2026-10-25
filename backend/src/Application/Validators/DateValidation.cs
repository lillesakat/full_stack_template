namespace Application.Validators;

// Fælles regel for datointervaller: begge datoer er påkrævede, og slutdatoen skal være efter startdatoen.
// En manglende dato modtages som DateOnly's standardværdi, da DateOnly ikke kan være null.
public static class DateValidation
{
    public static void Validate(DateOnly startDate, DateOnly endDate, IDictionary<string, string> errors)
    {
        if (startDate == default)
            errors["startDate"] = "Startdato er påkrævet.";

        if (endDate == default)
            errors["endDate"] = "Slutdato er påkrævet.";
        else if (startDate >= endDate)
            errors["endDate"] = "Slutdato skal være efter startdato.";
    }
}