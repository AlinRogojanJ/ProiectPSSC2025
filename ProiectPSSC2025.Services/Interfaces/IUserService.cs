using ProiectPSSC2025.Models.DTOs;
using ProiectPSSC2025.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<UserDTO?> GetUserByIdAsync(string id);
        Task AddUserAsync(User user);
    }
}
