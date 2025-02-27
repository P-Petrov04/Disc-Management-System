using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class CreateUserModel
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(20)]
        public string Password { get; set; }

        [MaxLength(15)]
        public string PhoneNumber { get; set; }
    }
}
