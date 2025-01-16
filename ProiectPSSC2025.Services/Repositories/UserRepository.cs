using ProiectPSSC2025.Models.Contexts;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ProiectPSSC2025.Services.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ReservationContext _reservationContext;

        public UserRepository(ReservationContext reservationContext)
        {
            _reservationContext = reservationContext;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _reservationContext.Users.ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _reservationContext.Users.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddUserAsync(User user)
        {
            await _reservationContext.Users.AddAsync(user);
            await _reservationContext.SaveChangesAsync();
        }
    }
}
