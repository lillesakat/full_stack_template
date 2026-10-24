using Domain.Entities;

namespace Application.DTOs;

public record PeriodDto(int Id, string Name, DateOnly StartDate, DateOnly EndDate)
{
    public static PeriodDto FromEntity(Period period)
        => new(period.Id, period.Name, period.StartDate, period.EndDate);
}

// Bruges både til opret og ret, da felterne er de samme
public record SavePeriodRequest(string? Name, DateOnly StartDate, DateOnly EndDate);
