using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectPSSC2025.Models
{
    public class Room
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public int Number { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public float PricePerNight { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }
    }
}
