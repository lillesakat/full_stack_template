using Domain.Entities;

namespace Application.DTOs;

public record StaffGroupDto(int Id, string Name)
{
    public static StaffGroupDto FromEntity(StaffGroup staffGroup)
        => new(staffGroup.Id, staffGroup.Name);
}

// Bruges til både oprettelse og redigering.
public record SaveStaffGroupRequest(string? Name);