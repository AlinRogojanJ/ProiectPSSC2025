using ProiectPSSC2025.DTOs;

namespace ProiectPSSC2025.Interfaces
{
    public interface IReservationService
    {
        Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync();
        Task<ReservationDTO> GetReservationByIdAsync(string id);
        Task CreateReservationAsync(ReservationDTO reservationDto);
    }
}
