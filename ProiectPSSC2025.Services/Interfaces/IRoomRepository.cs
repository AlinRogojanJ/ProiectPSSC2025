using ProiectPSSC2025.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Interfaces
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<IEnumerable<Room>> GetAllAvailableRoomsAsync();
        Task<Room> GetRoomByIdAsync(string id);
        Task CreateRoomAsync(Room room);
        Task DeleteRoomAsync(string id);
        Task UpdateRoomAsync(Room room);
    }
}
