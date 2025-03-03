using System;
using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class UpdateDiscModel
    {
        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(100)]
        public string? Artist { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [MaxLength(20)]
        public string? Format { get; set; }

        public int? DurationMinutes { get; set; }

        public IFormFile? Photo { get; set; }
    }
}
