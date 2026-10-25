using Application.DTOs;

namespace Application.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<UserDto>> GetAllAsync();
    Task<UserDto> GetByIdAsync(int id);
    Task<UserDto> CreateAsync(SaveUserRequest request);
    Task<UserDto> UpdateAsync(int id, SaveUserRequest request);
    Task DeleteAsync(int id);
}