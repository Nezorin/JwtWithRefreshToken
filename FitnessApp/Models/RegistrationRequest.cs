using System.ComponentModel.DataAnnotations;

namespace API.Models
{
    public class RegistrationRequest
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
        public DateOnly Birthday { get; set; }
    }
}
