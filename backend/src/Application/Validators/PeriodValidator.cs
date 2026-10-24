using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class PeriodValidator
{
    private const int NameMaxLength = 100;
    private readonly IAppDbContext _db;

    public PeriodValidator(IAppDbContext db)
    {
        _db = db;
    }


    // Kaster en ValidationException med alle fejl på én gang.
    // excludeId er perioden selv ved redigering, så den ikke overlapper med sig selv.
    public async Task ValidateAsync(SavePeriodRequest request, int? excludeId)
    {
        var errors = new Dictionary<string, string>();

        ValidateName(request.Name, errors);
        ValidateDates(request.StartDate, request.EndDate, errors);

        var datesAreValid = !errors.ContainsKey("startDate") && !errors.ContainsKey("endDate");
        if (datesAreValid)
            await ValidateNoOverlapAsync(request.StartDate, request.EndDate, excludeId, errors);

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    private static void ValidateName(string? name, Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(name))
            errors["name"] = "Navn er påkrævet.";
        else if (name.Trim().Length > NameMaxLength)
            errors["name"] = $"Navn må højst være {NameMaxLength} tegn.";
    }


    // En manglende dato modtages som DateOnly's standardværdi, da DateOnly ikke kan være null
    private static void ValidateDates(DateOnly startDate, DateOnly endDate, Dictionary<string, string> errors)
    {
        if (startDate == default)
            errors["startDate"] = "Startdato er påkrævet.";

        if (endDate == default)
            errors["endDate"] = "Slutdato er påkrævet.";
        else if (startDate >= endDate)
            errors["endDate"] = "Slutdato skal være efter startdato.";
    }


    // Sammenligner med de øvrige perioder via DateRange.Overlaps, så overlap-reglen kun er defineret ét sted. 
    private async Task ValidateNoOverlapAsync(DateOnly startDate, DateOnly endDate, int? excludeId, Dictionary<string, string> errors)
    {
        var otherPeriods = await _db.Periods
            .AsNoTracking()
            .Where(p => p.Id != excludeId)
            .ToListAsync();

        var overlapping = otherPeriods.FirstOrDefault(p =>
            DateRange.Overlaps(startDate, endDate, p.StartDate, p.EndDate));

        if (overlapping is not null)
            errors["startDate"] = $"Perioden overlapper med \"{overlapping.Name}\" " +
                                  $"({overlapping.StartDate:dd-MM-yyyy} til {overlapping.EndDate:dd-MM-yyyy}).";
    }
}