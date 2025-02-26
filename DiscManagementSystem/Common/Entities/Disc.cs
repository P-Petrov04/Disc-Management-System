using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Disc
    {
        [Key]
        public int DiscId { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [MaxLength (100)]
        public string? Artist { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Format { get; set; } // CD, Vinyl, Cassette
        public bool IsAvailable { get; set; } = true;
        public int DurationMinutes { get; set; } // Duration in minutes

        public ICollection<Rental> Rentals { get; set; }
    }
}
