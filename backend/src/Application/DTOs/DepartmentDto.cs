using Domain.Entities;

namespace Application.DTOs;

public record DepartmentDto(int Id, string Name, string Color)
{
    public static DepartmentDto FromEntity(Department department)
        => new(department.Id, department.Name, department.Color);
}

// Bruges til både oprettelse og redigering.
public record SaveDepartmentRequest(string? Name, string? Color);