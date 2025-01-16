using ProiectPSSC2025.DTOs;
using System.Runtime.InteropServices;

namespace ProiectPSSC2025.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();

        Task PublishReservationAsync(ReservationDTO reservationDTO);

        Task<ReservationDTO> GetReservationByIdAsync(string id);
        Task CreateReservationAsync(ReservationDTO reservationDto);
        Task RemoveReservationAsync(string id);
        Task UpdateReservationAsync(ReservationDTO reservationDto);
    }
}
