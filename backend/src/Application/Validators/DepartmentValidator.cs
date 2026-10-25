using Application.DTOs;
using Application.Exceptions;
using Domain.Rules;

namespace Application.Validators;

public class DepartmentValidator
{
    public void Validate(SaveDepartmentRequest request)
    {
        var errors = new Dictionary<string, string>();

        NameValidation.Validate(request.Name, errors);

        if (!HexColor.IsValid(request.Color?.Trim()))
            errors["color"] = "Farve skal være en gyldig HEX-kode, fx #FF0000.";

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}