using Application.DTOs;

namespace Application.Interfaces;

public interface IPeriodService
{
    Task<IReadOnlyList<PeriodDto>> GetAllAsync();
    Task<PeriodDto> GetByIdAsync(int id);
    Task<PeriodDto> CreateAsync(SavePeriodRequest request);
    Task<PeriodDto> UpdateAsync(int id, SavePeriodRequest request);
    Task DeleteAsync(int id);

}
