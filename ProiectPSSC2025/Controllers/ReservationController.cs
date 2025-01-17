using Microsoft.AspNetCore.Mvc;
using ProiectPSSC2025.DTOs;
using ProiectPSSC2025.Interfaces;
using ProiectPSSC2025.Models.DTOs;

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
        public async Task<IActionResult> Create([FromBody] RoomReservationRequestDTO reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reservationService.CreateReservationAsync(reservationDto);
            return Created();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Remove([FromRoute] string id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reservationService.RemoveReservationAsync(id);
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateReservationAsync([FromBody] ReservationDTO reservationDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reservationService.UpdateReservationAsync(reservationDTO);

            return Ok();
        }
    }
}
