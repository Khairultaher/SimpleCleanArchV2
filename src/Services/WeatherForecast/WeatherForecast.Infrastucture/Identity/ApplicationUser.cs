
using Microsoft.AspNetCore.Identity;
using System;

namespace WeatherForecast.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<string>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? MiddleName { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }

    public class ApplicationRole : IdentityRole<string>
    {
        public ApplicationRole() : base()
        {
        }

        public ApplicationRole(string roleName) : base(roleName)
        {
        }
        public ApplicationRole(string id, string roleName, string displayName) : base(roleName)
        {
            Id = id;
            DisplayName = displayName;
            Name = roleName;
        }
        public string? DisplayName { get; set; }
    }
}
