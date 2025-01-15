using System.ComponentModel.DataAnnotations;

namespace ProiectPSSC2025.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string Email { get; set; }
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }
    }
}
