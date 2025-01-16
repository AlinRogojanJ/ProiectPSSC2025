using AutoMapper;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;

public class ReservationService : IReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IMapper _mapper;

    public ReservationService(IReservationRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReservationDTO>> GetAllReservationsAsync()
    {
        var reservations = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ReservationDTO>>(reservations);
    }

    public async Task<ReservationDTO> GetReservationByIdAsync(string id)
    {
        var reservation = await _repository.GetByIdAsync(id);
        return _mapper.Map<ReservationDTO>(reservation);
    }

    public async Task CreateReservationAsync(ReservationDTO reservationDto)
    {
        var reservation = _mapper.Map<Reservation>(reservationDto);
        await _repository.AddAsync(reservation);
    }
}