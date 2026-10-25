using Domain.Entities;
using System.Linq.Expressions;

namespace Application.DTOs;

public record StaffGroupDto(int Id, string Name, int EmploymentCount)
{
    // Bruges direkte i databaseforespørgsler, så antallet af ansættelser tælles af databasen.
    public static Expression<Func<StaffGroup, StaffGroupDto>> Projection =>
        staffGroup => new StaffGroupDto(staffGroup.Id, staffGroup.Name, staffGroup.Employments.Count);
}

// Bruges til både oprettelse og redigering.
public record SaveStaffGroupRequest(string? Name);