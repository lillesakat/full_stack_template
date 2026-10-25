using Application.DTOs;

namespace Application.Interfaces;

public interface IStaffGroupService
{
    Task<IReadOnlyList<StaffGroupDto>> GetAllAsync();
    Task<StaffGroupDto> GetByIdAsync(int id);
    Task<StaffGroupDto> CreateAsync(SaveStaffGroupRequest request);
    Task<StaffGroupDto> UpdateAsync(int id, SaveStaffGroupRequest request);
    Task DeleteAsync(int id);
}