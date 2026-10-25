using Application.DTOs;

namespace Application.Interfaces;

public interface IEmploymentService
{
    Task<IReadOnlyList<EmploymentDto>> GetAllAsync();
    Task<EmploymentDto> GetByIdAsync(int id);
    Task<EmploymentDto> CreateAsync(SaveEmploymentRequest request);
    Task<EmploymentDto> UpdateAsync(int id, SaveEmploymentRequest request);
    Task DeleteAsync(int id);
}