using System.ComponentModel.DataAnnotations;

namespace ProiectPSSC2025.DTOs
{
    public class ReservationDTO
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
