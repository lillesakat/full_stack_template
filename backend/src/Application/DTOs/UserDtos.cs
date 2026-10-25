using Domain.Entities;

namespace Application.DTOs;

public record UserDto(int Id, string Name, string Email, bool IsAdmin, bool IsStaff)
{
    public static UserDto FromEntity(User user)
        => new(user.Id, user.Name, user.Email, user.IsAdmin, user.IsStaff);
}

// Bruges til både oprettelse og redigering. Rollerne er nullable, så en manglende værdi
// kan skelnes fra "nej" og give en fejl, da alle felter er påkrævede.
public record SaveUserRequest(string? Name, string? Email, bool? IsAdmin, bool? IsStaff);