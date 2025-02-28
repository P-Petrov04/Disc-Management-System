using System.ComponentModel.DataAnnotations;

namespace API.ViewModel
{
    public class UpdateUserModel
    {

        [MaxLength(50)]
        public string? FirstName { get; set; }

        [MaxLength(50)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(20)]
        public string? Password { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
