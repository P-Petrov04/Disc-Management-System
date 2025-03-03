using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class UpdateRentalModel
    {
        public DateTime? ReturnDate { get; set; }

        [MaxLength(20)]
        public string? Status { get; set; } // Active, Returned

        public bool? IsRentalExceeded { get; set; }
    }
}
