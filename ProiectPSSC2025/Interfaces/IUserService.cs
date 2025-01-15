using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Models;

namespace ProiectPSSC2025.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(string id);
        Task AddUserAsync(User user);
    }
}
