using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class EmploymentService : IEmploymentService
{
    private readonly IAppDbContext _db;
    private readonly EmploymentValidator _validator;

    public EmploymentService(IAppDbContext db, EmploymentValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<EmploymentDto>> GetAllAsync()
    {
        var employments = await EmploymentsWithDetails()
            .AsNoTracking()
            .OrderBy(e => e.User.Name)
            .ThenBy(e => e.StartDate)
            .ToListAsync();

        return employments.Select(EmploymentDto.FromEntity).ToList();
    }

    public async Task<EmploymentDto> GetByIdAsync(int id)
    {
        var employment = await EmploymentsWithDetails()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id)
            ?? throw new NotFoundException($"Ansættelsen med id {id} findes ikke.");

        return EmploymentDto.FromEntity(employment);
    }

    public async Task<EmploymentDto> CreateAsync(SaveEmploymentRequest request)
    {
        await _validator.ValidateAsync(request, employmentId: null);

        var employment = new Employment();
        ApplyRequest(employment, request);

        _db.Employments.Add(employment);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(employment.Id);
    }

    public async Task<EmploymentDto> UpdateAsync(int id, SaveEmploymentRequest request)
    {
        var employment = await FindOrThrowAsync(id);
        await _validator.ValidateAsync(request, employmentId: id);

        ApplyRequest(employment, request);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var employment = await FindOrThrowAsync(id);

        _db.Employments.Remove(employment);
        await _db.SaveChangesAsync();
    }

    private IQueryable<Employment> EmploymentsWithDetails()
    {
        return _db.Employments
            .Include(e => e.User)
            .Include(e => e.Department)
            .Include(e => e.StaffGroup);
    }

    private static void ApplyRequest(Employment employment, SaveEmploymentRequest request)
    {
        employment.UserId = request.UserId!.Value;
        employment.DepartmentId = request.DepartmentId!.Value;
        employment.StaffGroupId = request.StaffGroupId!.Value;
        employment.HoursPerWeek = request.HoursPerWeek!.Value;
        employment.StartDate = request.StartDate!.Value;
        employment.EndDate = request.EndDate!.Value;
    }

    private async Task<Employment> FindOrThrowAsync(int id)
    {
        return await _db.Employments.FindAsync(id)
            ?? throw new NotFoundException($"Ansættelsen med id {id} findes ikke.");
    }
}