using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class StaffService : IStaffService
{
    private readonly IAppDbContext _db;

    public StaffService(IAppDbContext db)
    {
        _db = db;
    }

    // Returnerer én række pr. ansættelse, der overlapper perioden.
    // Betingelsen svarer til DateRange.Overlaps, men står i forespørgslen, så databasen filtrerer.
    public async Task<IReadOnlyList<StaffMemberDto>> GetForPeriodAsync(int periodId)
    {
        var period = await _db.Periods
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == periodId)
            ?? throw new NotFoundException($"Perioden med id {periodId} findes ikke.");

        var employments = await _db.Employments
            .AsNoTracking()
            .Include(e => e.User)
            .Include(e => e.Department)
            .Include(e => e.StaffGroup)
            .Where(e => e.StartDate <= period.EndDate && period.StartDate <= e.EndDate)
            .OrderBy(e => e.User.Name)
            .ToListAsync();

        return employments.Select(StaffMemberDto.FromEntity).ToList();
    }
}