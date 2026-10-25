using Application.DTOs;
using Application.Exceptions;

namespace Application.Validators;

public class StaffGroupValidator
{
    public void Validate(SaveStaffGroupRequest request)
    {
        var errors = new Dictionary<string, string>();

        NameValidation.Validate(request.Name, errors);

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }
}