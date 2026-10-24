using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class PeriodService : IPeriodService
{
    private readonly IAppDbContext _db;
    private readonly PeriodValidator _validator;

    public PeriodService(IAppDbContext db, PeriodValidator validator)
    {
        _db = db;
        _validator = validator;
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
        await _validator.ValidateAsync(request, excludeId: null);

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
        await _validator.ValidateAsync(request, excludeId: id);

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

}