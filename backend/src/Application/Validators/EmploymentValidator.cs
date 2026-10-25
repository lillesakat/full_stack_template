using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class EmploymentValidator
{
    private const int MinHoursPerWeek = 0;
    private const int MaxHoursPerWeek = 37;
    private readonly IAppDbContext _db;

    public EmploymentValidator(IAppDbContext db)
    {
        _db = db;
    }

    // Kaster en ValidationException med alle fejl på én gang.
    // employmentId er ansættelsen selv ved redigering og null ved oprettelse.
    public async Task ValidateAsync(SaveEmploymentRequest request, int? employmentId)
    {
        var errors = new Dictionary<string, string>();

        await ValidateUserAsync(request.UserId, errors);
        await ValidateDepartmentAsync(request.DepartmentId, errors);
        await ValidateStaffGroupAsync(request.StaffGroupId, errors);
        ValidateHours(request.HoursPerWeek, errors);
        DateValidation.Validate(request.StartDate, request.EndDate, errors);

        var canCheckOverlap = !errors.ContainsKey("userId")
                              && !errors.ContainsKey("startDate")
                              && !errors.ContainsKey("endDate");
        if (canCheckOverlap)
            await ValidateNoOverlapAsync(request.UserId!.Value, request.StartDate!.Value, request.EndDate!.Value, employmentId, errors);

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    private async Task ValidateUserAsync(int? userId, Dictionary<string, string> errors)
    {
        if (userId is null)
        {
            errors["userId"] = "Vælg en person.";
            return;
        }

        var user = await _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == userId);

        if (user is null)
            errors["userId"] = "Personen findes ikke.";
        else if (!user.IsStaff)
            errors["userId"] = $"{user.Name} har ikke rollen Personale og kan derfor ikke få en ansættelse.";
    }

    private async Task ValidateDepartmentAsync(int? departmentId, Dictionary<string, string> errors)
    {
        if (departmentId is null)
            errors["departmentId"] = "Vælg et afsnit.";
        else if (!await _db.Departments.AnyAsync(d => d.Id == departmentId))
            errors["departmentId"] = "Afsnittet findes ikke.";
    }

    private async Task ValidateStaffGroupAsync(int? staffGroupId, Dictionary<string, string> errors)
    {
        if (staffGroupId is null)
            errors["staffGroupId"] = "Vælg en personalegruppe.";
        else if (!await _db.StaffGroups.AnyAsync(s => s.Id == staffGroupId))
            errors["staffGroupId"] = "Personalegruppen findes ikke.";
    }

    private static void ValidateHours(int? hoursPerWeek, Dictionary<string, string> errors)
    {
        if (hoursPerWeek is null)
            errors["hoursPerWeek"] = "Timer pr. uge er påkrævet.";
        else if (hoursPerWeek < MinHoursPerWeek || hoursPerWeek > MaxHoursPerWeek)
            errors["hoursPerWeek"] = $"Timer pr. uge skal være et helt tal fra {MinHoursPerWeek} til {MaxHoursPerWeek}.";
    }

    // Sammenligner med personens øvrige ansættelser via DateRange.Overlaps, så overlap-reglen kun er defineret ét sted.
    private async Task ValidateNoOverlapAsync(int userId, DateOnly startDate, DateOnly endDate, int? employmentId, Dictionary<string, string> errors)
    {
        var otherEmployments = await _db.Employments
            .AsNoTracking()
            .Include(e => e.Department)
            .Where(e => e.UserId == userId && e.Id != employmentId)
            .ToListAsync();

        var overlapping = otherEmployments.FirstOrDefault(e =>
            DateRange.Overlaps(startDate, endDate, e.StartDate, e.EndDate));

        if (overlapping is not null)
            errors["startDate"] = $"Personen har allerede en ansættelse på {overlapping.Department.Name} " +
                                  $"fra {overlapping.StartDate:dd-MM-yyyy} til {overlapping.EndDate:dd-MM-yyyy}.";
    }
}