using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models;
using AutoMapper;

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

    public async Task RemoveReservationAsync(string id)
    {
        await _repository.RemoveAsync(id);
    }

    public async Task UpdateReservationAsync(ReservationDTO reservationDto)
    {
        try
        {
            var newReservation = new Reservation() 
            { 
                UpdatedDate = DateTime.Now,
                StartDate = reservationDto.StartDate,
                EndDate = reservationDto.EndDate,
                RoomId = reservationDto.RoomId,
                Id = reservationDto.Id,
                UserId = reservationDto.UserId,
                CreatedDate = reservationDto.CreatedDate,
            };

            await _repository.UpdateAsync(newReservation);

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}