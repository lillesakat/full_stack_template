using Domain.Entities;
using System.Linq.Expressions;

namespace Application.DTOs;

public record DepartmentDto(int Id, string Name, string Color, int EmploymentCount)
{
    // Bruges direkte i databaseforespørgsler, så antallet af ansættelser tælles af databasen.
    public static Expression<Func<Department, DepartmentDto>> Projection =>
        department => new DepartmentDto(department.Id, department.Name, department.Color, department.Employments.Count);
}

// Bruges til både oprettelse og redigering.
public record SaveDepartmentRequest(string? Name, string? Color);