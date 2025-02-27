using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string? LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string? Password { get; set; }

        [MaxLength(15)]
        public string? PhoneNumber { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        public ICollection<Rental> Rentals { get; set; }
    }
}
