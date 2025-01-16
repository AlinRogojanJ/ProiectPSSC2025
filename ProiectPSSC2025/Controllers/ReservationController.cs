using Microsoft.AspNetCore.Mvc;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;

namespace ProiectPSSC2025.Controllers
{   // Controller
        [ApiController]
        [Route("api/[controller]")]
        public class ReservationsController : ControllerBase
        {
            private readonly IReservationService _reservationService;

            public ReservationsController(IReservationService reservationService)
            {
                _reservationService = reservationService;
            }

            [HttpGet]
            public async Task<IActionResult> GetAll()
            {
                var reservations = await _reservationService.GetAllReservationsAsync();
                if (reservations == null)
                    return NotFound();
                return Ok(reservations);
            }

            [HttpGet("{id}")]
            public async Task<IActionResult> GetById(string id)
            {
                var reservation = await _reservationService.GetReservationByIdAsync(id);
                if (reservation == null)
                    return NotFound();
                return Ok(reservation);
            }

            [HttpPost]
            public async Task<IActionResult> Create([FromBody] ReservationDTO reservationDto)
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _reservationService.CreateReservationAsync(reservationDto);
                return Created("Reservation created", reservationDto);
            }
        }
}
