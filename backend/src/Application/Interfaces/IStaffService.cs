using Application.DTOs;

namespace Application.Interfaces;

public interface IStaffService
{
    Task<IReadOnlyList<StaffMemberDto>> GetForPeriodAsync(int periodId);
}