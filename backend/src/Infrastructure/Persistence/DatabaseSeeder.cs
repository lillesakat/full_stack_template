using Domain.Entities;

namespace Infrastructure.Persistence;

public static class DatabaseSeeder
{
    // Indsætter eksempeldata fra opgaven, men kun når databasen er helt tom,
    // så data, som brugeren selv har oprettet, aldrig bliver overskrevet.
    public static void Seed(AppDbContext db)
    {
        var isEmpty = !db.Periods.Any() && !db.Departments.Any() && !db.StaffGroups.Any() && !db.Users.Any();
        if (!isEmpty)
            return;

        var emergencyRoom = new Department { Name = "Skadestue", Color = "#FF0000" };
        var wardA = new Department { Name = "Sengeafsnit A", Color = "#00FF00" };

        var seniorPhysician = new StaffGroup { Name = "Overlæge" };
        var specialistPhysician = new StaffGroup { Name = "Afdelingslæge" };
        var juniorPhysician = new StaffGroup { Name = "Introlæge" };

        var anna = new User { Name = "Anna Hansen", Email = "anna.hansen@hospital.dk", IsAdmin = true, IsStaff = true };
        var bo = new User { Name = "Bo Nielsen", Email = "bo.nielsen@hospital.dk", IsStaff = true };
        var clara = new User { Name = "Clara Jensen", Email = "clara.jensen@hospital.dk", IsStaff = true };
        var karen = new User { Name = "Karen Larsen", Email = "karen.larsen@hospital.dk", IsAdmin = true };

        db.Departments.AddRange(emergencyRoom, wardA);
        db.StaffGroups.AddRange(seniorPhysician, specialistPhysician, juniorPhysician);
        db.Users.AddRange(anna, bo, clara, karen);

        db.Periods.AddRange(
            new Period { Name = "December 2026", StartDate = new DateOnly(2026, 12, 1), EndDate = new DateOnly(2026, 12, 31) },
            new Period { Name = "Januar 2027", StartDate = new DateOnly(2027, 1, 1), EndDate = new DateOnly(2027, 1, 31) });

        db.Employments.AddRange(
            new Employment { User = anna, Department = emergencyRoom, StaffGroup = seniorPhysician, HoursPerWeek = 37, StartDate = new DateOnly(2026, 1, 1), EndDate = new DateOnly(2027, 12, 31) },
            new Employment { User = bo, Department = emergencyRoom, StaffGroup = specialistPhysician, HoursPerWeek = 37, StartDate = new DateOnly(2026, 6, 1), EndDate = new DateOnly(2026, 12, 15) },
            new Employment { User = bo, Department = wardA, StaffGroup = specialistPhysician, HoursPerWeek = 30, StartDate = new DateOnly(2026, 12, 16), EndDate = new DateOnly(2027, 6, 30) },
            new Employment { User = clara, Department = wardA, StaffGroup = juniorPhysician, HoursPerWeek = 37, StartDate = new DateOnly(2027, 1, 1), EndDate = new DateOnly(2027, 12, 31) });

        db.SaveChanges();
    }
}