using Domain.Entities;

namespace Application.DTOs;

public record PeriodDto(int Id, string Name, DateOnly StartDate, DateOnly EndDate)
{
    public static PeriodDto FromEntity(Period period)
        => new(period.Id, period.Name, period.StartDate, period.EndDate);
}

// Bruges til både oprettelse og redigering. Datoerne er nullable, så en manglende dato
// giver en dansk fejlbesked fra validatoren i stedet for en teknisk fejl.
public record SavePeriodRequest(string? Name, DateOnly? StartDate, DateOnly? EndDate);
