namespace Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;

    public List<Employment> Employments { get; set; } = new();

}
