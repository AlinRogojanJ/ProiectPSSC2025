using Microsoft.EntityFrameworkCore;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using ProiectPSSC2025.Data;

public class ReservationRepository : IReservationRepository
{
    private readonly AppDbContext _context;

    public ReservationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Reservation>> GetAllAsync()
    {
        return await _context.Set<Reservation>().ToListAsync();
    }

    public async Task<Reservation> GetByIdAsync(string id)
    {
        return await _context.Set<Reservation>().FindAsync(id);
    }

    public async Task AddAsync(Reservation reservation)
    {
        await _context.Set<Reservation>().AddAsync(reservation);
        await _context.SaveChangesAsync();
    }
}