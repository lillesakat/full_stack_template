using Domain.Entities;

namespace Application.DTOs;

public record EmploymentDto(
    int Id,
    int UserId,
    string UserName,
    int DepartmentId,
    string DepartmentName,
    string DepartmentColor,
    int StaffGroupId,
    string StaffGroupName,
    int HoursPerWeek,
    DateOnly StartDate,
    DateOnly EndDate)
{
    // Kræver at User, Department og StaffGroup er indlæst på entiteten.
    public static EmploymentDto FromEntity(Employment employment)
        => new(
            employment.Id,
            employment.UserId,
            employment.User.Name,
            employment.DepartmentId,
            employment.Department.Name,
            employment.Department.Color,
            employment.StaffGroupId,
            employment.StaffGroup.Name,
            employment.HoursPerWeek,
            employment.StartDate,
            employment.EndDate);
}

// Bruges til både oprettelse og redigering. Alle felter er nullable, så en manglende værdi
// kan skelnes fra fx 0 timer og give en dansk fejlbesked.
public record SaveEmploymentRequest(
    int? UserId,
    int? DepartmentId,
    int? StaffGroupId,
    int? HoursPerWeek,
    DateOnly? StartDate,
    DateOnly? EndDate);