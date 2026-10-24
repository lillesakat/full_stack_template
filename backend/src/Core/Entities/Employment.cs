namespace Domain.Entities;

public class Employment
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int DepartmentId { get; set; }
    public Department Department { get; set; } = null!;

    public int StaffGroupId { get; set; }
    public StaffGroup StaffGroup { get; set; } = null!;


    public int HoursPerWeek { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

}
