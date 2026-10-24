using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PeriodService : IPeriodService
{
    private const int NameMaxLength = 100;
    private readonly IAppDbContext _db;

    public PeriodService(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<PeriodDto>> GetAllAsync()
    {
        var periods = await _db.Periods
            .AsNoTracking()
            .OrderBy(p => p.StartDate)
            .ToListAsync();

        return periods.Select(PeriodDto.FromEntity).ToList();
    }

    public async Task<PeriodDto> GetByIdAsync(int id)
    {
        var period = await FindOrThrowAsync(id);
        return PeriodDto.FromEntity(period);
    }

    public async Task<PeriodDto> CreateAsync(SavePeriodRequest request)
    {
        await ValidateAsync(request, excludeId: null);

        var period = new Period
        {
            Name = request.Name!.Trim(),
            StartDate = request.StartDate,
            EndDate = request.EndDate
        };

        _db.Periods.Add(period);
        await _db.SaveChangesAsync();

        return PeriodDto.FromEntity(period);
    }

    public async Task<PeriodDto> UpdateAsync(int id, SavePeriodRequest request)
    {
        var period = await FindOrThrowAsync(id);
        await ValidateAsync(request, excludeId: id);

        period.Name = request.Name!.Trim();
        period.StartDate = request.StartDate;
        period.EndDate = request.EndDate;

        await _db.SaveChangesAsync();

        return PeriodDto.FromEntity(period);
    }

    public async Task DeleteAsync(int id)
    {
        var period = await FindOrThrowAsync(id);

        _db.Periods.Remove(period);
        await _db.SaveChangesAsync();
    }

    private async Task<Period> FindOrThrowAsync(int id)
    {
        return await _db.Periods.FindAsync(id)
            ?? throw new NotFoundException($"Perioden med id {id} findes ikke.");
    }


    // Samler alle fejl, før der kastes, så brugeren ser alle problemer på én gang.
    private async Task ValidateAsync(SavePeriodRequest request, int? excludeId)
    {
        var errors = new Dictionary<string, string>();

        if (string.IsNullOrWhiteSpace(request.Name))
            errors["name"] = "Navn er påkrævet.";
        else if (request.Name.Trim().Length > NameMaxLength)
            errors["name"] = $"Navn må højst være {NameMaxLength} tegn.";


        if (request.StartDate == default)
            errors["startDate"] = "Startdato er påkrævet.";
        if (request.EndDate == default)
            errors["endDate"] = "Slutdato er påkrævet.";

        if (!errors.ContainsKey("startDate") && !errors.ContainsKey("endDate"))
        {
            if (request.StartDate >= request.EndDate)
            {
                errors["endDate"] = "Slutdato skal være efter startdato.";
            }
            else
            {

                var otherPeriods = await _db.Periods
                    .AsNoTracking()
                    .Where(p => p.Id != excludeId)
                    .ToListAsync();

                var overlapping = otherPeriods.FirstOrDefault(p =>
                    DateRange.Overlaps(request.StartDate, request.EndDate, p.StartDate, p.EndDate));

                if (overlapping is not null)
                    errors["startDate"] = $"Perioden overlapper med \"{overlapping.Name}\" " +
                                          $"({overlapping.StartDate:dd-MM-yyyy} til {overlapping.EndDate:dd-MM-yyyy}).";
            }
        }

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}