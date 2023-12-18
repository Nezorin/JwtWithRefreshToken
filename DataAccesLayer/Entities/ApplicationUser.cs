using Microsoft.AspNetCore.Identity;

namespace DataAccesLayer.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public DateOnly? Birthday { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
