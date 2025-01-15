using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectPSSC2025.Models
{
    public class Reservation
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        [Required]
        [ForeignKey("Room")]
        public string RoomId { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public DateTime UpdatedDate { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [Required]
        public string Status { get; set; }

        public User? User { get; set; }
        public Room? Room { get; set; }
    }
}
