namespace Domain.Rules;

public static class DateRange
{
    // Afgør om to dato intervaller overlapper hinande. Datoerne er inklusive.
    public static bool Overlaps(DateOnly startA, DateOnly endA, DateOnly startB, DateOnly endB)
        => startA <= endB && startB <= endA;

}
