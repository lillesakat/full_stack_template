using Application.DTOs;

namespace Application.Interfaces;

public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto> GetByIdAsync(int id);
    Task<DepartmentDto> CreateAsync(SaveDepartmentRequest request);
    Task<DepartmentDto> UpdateAsync(int id, SaveDepartmentRequest request);
    Task DeleteAsync(int id);
}