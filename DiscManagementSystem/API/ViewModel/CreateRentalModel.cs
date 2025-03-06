using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class CreateRentalModel
    {
        [Required]
        public int DiscId { get; set; }

        [Required]
        public DateTime RentalDate { get; set; }

        [Required]
        public int RentalDuration { get; set; } // Duration in days

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } // Active, Returned
    }
}
