using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Application.Validators;

public class UserValidator
{
    private readonly IAppDbContext _db;

    public UserValidator(IAppDbContext db)
    {
        _db = db;
    }

    // Kaster en ValidationException med alle fejl på én gang.
    // userId er brugeren selv ved redigering og null ved oprettelse.
    public async Task ValidateAsync(SaveUserRequest request, int? userId)
    {
        var errors = new Dictionary<string, string>();

        NameValidation.Validate(request.Name, errors);
        await ValidateEmailAsync(request.Email, userId, errors);
        ValidateRoles(request.IsAdmin, request.IsStaff, errors);
        await ValidateStaffRoleIsKeptAsync(request.IsStaff, userId, errors);

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    private async Task ValidateEmailAsync(string? email, int? userId, Dictionary<string, string> errors)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            errors["email"] = "E-mail er påkrævet.";
            return;
        }

        if (!EmailAddress.IsValid(email.Trim()))
        {
            errors["email"] = "E-mail skal være en gyldig adresse, fx navn@hospital.dk.";
            return;
        }

        var normalizedEmail = EmailAddress.Normalize(email);
        var isTaken = await _db.Users.AnyAsync(u => u.Email == normalizedEmail && u.Id != userId);

        if (isTaken)
            errors["email"] = "E-mailen er allerede i brug.";
    }

    private static void ValidateRoles(bool? isAdmin, bool? isStaff, Dictionary<string, string> errors)
    {
        if (isAdmin is null)
            errors["isAdmin"] = "Angiv om brugeren er admin.";

        if (isStaff is null)
            errors["isStaff"] = "Angiv om brugeren er personale.";

        if (isAdmin == false && isStaff == false)
            errors["roles"] = "Brugeren skal have mindst én rolle.";
    }

    // En bruger med ansættelser skal beholde rollen Personale, da kun personale kan have ansættelser.
    private async Task ValidateStaffRoleIsKeptAsync(bool? isStaff, int? userId, Dictionary<string, string> errors)
    {
        if (userId is null || isStaff != false)
            return;

        var hasEmployments = await _db.Employments.AnyAsync(e => e.UserId == userId);

        if (hasEmployments)
            errors["isStaff"] = "Brugeren har ansættelser og skal derfor beholde rollen Personale.";
    }
}