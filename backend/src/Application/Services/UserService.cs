using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces;
using Application.Validators;
using Domain.Entities;
using Domain.Rules;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class UserService : IUserService
{
    private readonly IAppDbContext _db;
    private readonly UserValidator _validator;

    public UserService(IAppDbContext db, UserValidator validator)
    {
        _db = db;
        _validator = validator;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync()
    {
        var users = await _db.Users
            .AsNoTracking()
            .OrderBy(u => u.Name)
            .ToListAsync();

        return users.Select(UserDto.FromEntity).ToList();
    }

    public async Task<UserDto> GetByIdAsync(int id)
    {
        var user = await FindOrThrowAsync(id);
        return UserDto.FromEntity(user);
    }

    public async Task<UserDto> CreateAsync(SaveUserRequest request)
    {
        await _validator.ValidateAsync(request, userId: null);

        var user = new User();
        ApplyRequest(user, request);

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return UserDto.FromEntity(user);
    }

    public async Task<UserDto> UpdateAsync(int id, SaveUserRequest request)
    {
        var user = await FindOrThrowAsync(id);
        await _validator.ValidateAsync(request, userId: id);

        ApplyRequest(user, request);
        await _db.SaveChangesAsync();

        return UserDto.FromEntity(user);
    }

    public async Task DeleteAsync(int id)
    {
        var user = await FindOrThrowAsync(id);

        var employmentCount = await _db.Employments.CountAsync(e => e.UserId == id);
        if (employmentCount > 0)
            throw new ConflictException(
                $"Brugeren \"{user.Name}\" kan ikke slettes, fordi vedkommende har {employmentCount} ansættelse(r).");

        _db.Users.Remove(user);
        await _db.SaveChangesAsync();
    }

    private static void ApplyRequest(User user, SaveUserRequest request)
    {
        user.Name = request.Name!.Trim();
        user.Email = EmailAddress.Normalize(request.Email!);
        user.IsAdmin = request.IsAdmin == true;
        user.IsStaff = request.IsStaff == true;
    }

    private async Task<User> FindOrThrowAsync(int id)
    {
        return await _db.Users.FindAsync(id)
            ?? throw new NotFoundException($"Brugeren med id {id} findes ikke.");
    }
}