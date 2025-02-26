using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Rental
    {
        [Key]
        public int RentalId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int DiscId { get; set; }

        [Required]
        public DateTime RentalDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // Active, Returned

        [Required]
        public int RentalDuration { get; set; } // Duration in days

        [Required]
        public bool IsRentalExceeded { get; set; } = false;

        public User User { get; set; }
        public Disc Disc { get; set; }
    }
}
