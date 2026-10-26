namespace Application.Validators;

// Fælles regel for datointervaller: begge datoer er påkrævede, og slutdatoen skal være efter startdatoen.
public static class DateValidation
{
    public static void Validate(DateOnly? startDate, DateOnly? endDate, IDictionary<string, string> errors)
    {
        if (startDate is null)
            errors["startDate"] = "Startdato er påkrævet.";

        if (endDate is null)
            errors["endDate"] = "Slutdato er påkrævet.";
        else if (startDate >= endDate)
            errors["endDate"] = "Slutdato skal være efter startdato.";
    }
}