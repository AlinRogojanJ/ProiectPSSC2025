using ProiectPSSC2025.Models;

namespace ProiectPSSC2025.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task AddUserAsync(User user);
    }
}
