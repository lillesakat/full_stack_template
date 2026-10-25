using Domain.Entities;

namespace Application.DTOs;

public record StaffMemberDto(
    int EmploymentId,
    string Name,
    string DepartmentName,
    string DepartmentColor,
    string StaffGroupName,
    int HoursPerWeek)
{
    // Kræver at User, Department og StaffGroup er indlæst på entiteten.
    public static StaffMemberDto FromEntity(Employment employment)
        => new(
            employment.Id,
            employment.User.Name,
            employment.Department.Name,
            employment.Department.Color,
            employment.StaffGroup.Name,
            employment.HoursPerWeek);
}