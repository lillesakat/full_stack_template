using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IAppDbContext _db;
    private readonly DepartmentValidator _validator;

    public DepartmentService(IAppDbContext db, DepartmentValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync()
    {
        return await _db.Departments
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(DepartmentDto.Projection)
            .ToListAsync();
    }

    public async Task<DepartmentDto> GetByIdAsync(int id)
    {
        return await _db.Departments
            .AsNoTracking()
            .Where(d => d.Id == id)
            .Select(DepartmentDto.Projection)
            .FirstOrDefaultAsync()
            ?? throw new NotFoundException($"Afsnittet med id {id} findes ikke.");
    }

    public async Task<DepartmentDto> CreateAsync(SaveDepartmentRequest request)
    {
        _validator.Validate(request);

        var department = new Department
        {
            Name = request.Name!.Trim(),
            Color = request.Color!.Trim().ToUpperInvariant()
        };

        _db.Departments.Add(department);
        await _db.SaveChangesAsync();

        return await GetByIdAsync(department.Id);
    }

    public async Task<DepartmentDto> UpdateAsync(int id, SaveDepartmentRequest request)
    {
        var department = await FindOrThrowAsync(id);
        _validator.Validate(request);

        department.Name = request.Name!.Trim();
        department.Color = request.Color!.Trim().ToUpperInvariant();

        await _db.SaveChangesAsync();

        return await GetByIdAsync(id);
    }

    public async Task DeleteAsync(int id)
    {
        var department = await FindOrThrowAsync(id);

        var employmentCount = await _db.Employments.CountAsync(e => e.DepartmentId == id);
        if (employmentCount > 0)
            throw new ConflictException(
                $"Afsnittet \"{department.Name}\" kan ikke slettes, fordi det har {employmentCount} tilknyttede ansættelse(r).");

        _db.Departments.Remove(department);
        await _db.SaveChangesAsync();
    }

    private async Task<Department> FindOrThrowAsync(int id)
    {
        return await _db.Departments.FindAsync(id)
            ?? throw new NotFoundException($"Afsnittet med id {id} findes ikke.");
    }
}