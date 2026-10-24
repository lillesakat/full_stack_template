namespace Domain.Entities;

public class StaffGroup
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Employment> Employments { get; set; } = new();
}
