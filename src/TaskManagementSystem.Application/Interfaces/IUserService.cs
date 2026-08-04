using TaskManagementSystem.Application.DTOs.User;

namespace TaskManagementSystem.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetUserByIdAsync(string userId);
        Task<UserDto?> GetUserByEmailAsync(string email);
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<bool> UpdateUserAsync(string userId, UserDto userDto);
        Task<bool> DeleteUserAsync(string userId);
    }
}