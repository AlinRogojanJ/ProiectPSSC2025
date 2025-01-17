using Microsoft.EntityFrameworkCore;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Models.Contexts;
using ProiectPSSC2025.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Services.Repositories
{
    public class RoomRepository : IRoomRepository
    {

        private readonly ReservationContext _reservationContext;

        public RoomRepository(ReservationContext reservationContext)
        {
            _reservationContext = reservationContext;
        }

        public async Task CreateRoomAsync(Room room)
        {
            await _reservationContext.Rooms.AddAsync(room);
            await _reservationContext.SaveChangesAsync();
        }

        public async Task DeleteRoomAsync(string id)
        {
            var roomToDelete = await _reservationContext.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            if (roomToDelete != null)
            {
                _reservationContext.Rooms.Remove(roomToDelete);
                await _reservationContext.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Room>> GetAllAvailableRoomsAsync()
        {
            var availableRooms = await _reservationContext.Rooms.Where(r => r.Status == "Available").ToListAsync();
            return availableRooms;
        }

        public async Task<IEnumerable<Room>> GetAllRoomsAsync()
        {
            var allRooms = await _reservationContext.Rooms.ToListAsync();
            return allRooms;
        }

        public async Task<Room?> GetRoomByIdAsync(string id)
        {
            var roomWithSpecifiedId = await _reservationContext.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            return roomWithSpecifiedId;
        }

        public async Task UpdateRoomAsync(Room room)
        {
            var roomToUpdate = await _reservationContext.Rooms.FirstOrDefaultAsync( r => r.Id == room.Id);

            if (roomToUpdate != null)
            {
                roomToUpdate.PricePerNight = room.PricePerNight;
                roomToUpdate.Status = room.Status;
                roomToUpdate.Number = room.Number;
                roomToUpdate.Type = room.Type;
                
                await _reservationContext.SaveChangesAsync();
            }
        }

        public async Task UpdateRoomStatusAsync(string roomId, string roomStatus)
        {
            var roomToUpdate = await _reservationContext.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
            
            if (roomToUpdate != null)
            {
                roomToUpdate.Status = roomStatus;
                
                await _reservationContext.SaveChangesAsync();
            }
        }
    }
}
