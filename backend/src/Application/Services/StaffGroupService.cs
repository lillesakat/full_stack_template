using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class StaffGroupService : IStaffGroupService
{
    private readonly IAppDbContext _db;
    private readonly StaffGroupValidator _validator;

    public StaffGroupService(IAppDbContext db, StaffGroupValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<StaffGroupDto>> GetAllAsync()
    {
        return await _db.StaffGroups
            .AsNoTracking()
            .OrderBy(s => s.Name)
            .Select(StaffGroupDto.Projection)
            .ToListAsync();
    }

    public async Task<StaffGroupDto> GetByIdAsync(int id)
    {
        return await _db.StaffGroups
            .AsNoTracking()
            .Where(s => s.Id == id)
            .Select(StaffGroupDto.Projection)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException($"Personalegruppen med id {id} findes ikke.");
    }

    public async Task<StaffGroupDto> CreateAsync(SaveStaffGroupRequest request)
    {
        _validator.Validate(request);

        var staffGroup = new StaffGroup { Name = request.Name!.Trim() };

        _db.StaffGroups.Add(staffGroup);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(staffGroup.Id);
    }

    public async Task<StaffGroupDto> UpdateAsync(int id, SaveStaffGroupRequest request)
    {
        var staffGroup = await FindOrThrowAsync(id);
        _validator.Validate(request);

        staffGroup.Name = request.Name!.Trim();

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var staffGroup = await FindOrThrowAsync(id);

        var employmentCount = await _db.Employments.CountAsync(e => e.StaffGroupId == id);
        if (employmentCount > 0)
            throw new ConflictException(
                $"Personalegruppen \"{staffGroup.Name}\" kan ikke slettes, fordi den har {employmentCount} tilknyttede ansættelse(r).");

        _db.StaffGroups.Remove(staffGroup);
        await _db.SaveChangesAsync();
    }

    private async Task<StaffGroup> FindOrThrowAsync(int id)
    {
        return await _db.StaffGroups.FindAsync(id)
            ?? throw new NotFoundException($"Personalegruppen med id {id} findes ikke.");
    }
}