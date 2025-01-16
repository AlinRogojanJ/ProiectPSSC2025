using Microsoft.EntityFrameworkCore;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Models.Contexts;

public class ReservationRepository : IReservationRepository
{
    private readonly ReservationContext _reservationContext;

    public ReservationRepository(ReservationContext reservationContext)
    {
        _reservationContext = reservationContext;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _reservationContext.Set<Reservation>().ToListAsync();
    }

    public async Task<Reservation> GetByIdAsync(string id)
    {
        return await _reservationContext.Reservations.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task AddAsync(Reservation reservation)
    {
        await _reservationContext.Reservations.AddAsync(reservation);
        await _reservationContext.SaveChangesAsync();
    }
}