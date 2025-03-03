using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class CreateDiscModel
    {
        [Required]
        [MaxLength(100)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Artist { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Format { get; set; } // CD, Vinyl, Cassette
        public int DurationMinutes { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
