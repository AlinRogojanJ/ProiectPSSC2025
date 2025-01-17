using ProiectPSSC2025.Models;

namespace ProiectPSSC2025.Interfaces
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllAsync();
        Task<Reservation> GetByIdAsync(string id);
        Task AddAsync(Reservation reservation);
        Task RemoveAsync(string id);
        Task UpdateAsync(Reservation reservation);
    }
}
