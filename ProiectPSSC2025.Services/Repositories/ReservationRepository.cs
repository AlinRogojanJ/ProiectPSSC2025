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

    public async Task RemoveAsync(string id)
    {
        var reservationToBeDeleted = await _reservationContext.Reservations.FindAsync(id);

        if (reservationToBeDeleted != null)
        {
            _reservationContext.Reservations.Remove(reservationToBeDeleted);
        }

        await _reservationContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Reservation reservation)
    {
        var dbReservation = await _reservationContext.Reservations.FirstOrDefaultAsync(r => r.Id == reservation.Id);

        if (dbReservation != null)
        {
            // Update properties
            dbReservation.CreatedDate = reservation.CreatedDate;
            dbReservation.UpdatedDate = reservation.UpdatedDate;
            dbReservation.EndDate = reservation.EndDate;
            dbReservation.StartDate = reservation.StartDate;
            dbReservation.Status = reservation.Status;
            dbReservation.RoomId = reservation.RoomId;
            dbReservation.UserId = reservation.UserId;
        }

        await _reservationContext.SaveChangesAsync();
    }

}