using System;
using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class User
    {
        [Key] // Marks the property as the primary key
        public Guid Id { get; set; } = Guid.NewGuid(); // Default to a new Guid

        [Required] // Ensures that the Username is not null
        [StringLength(100)] // Limits the length of the Username
        public string Username { get; set; }

        [Required] // Ensures that the PasswordHash is not null
        public string PasswordHash { get; set; }

        [Required] // Ensures that the Email is not null
        [EmailAddress] // Validates that the string is a valid email format
        [StringLength(255)] // Limits the length of the Email
        public string Email { get; set; }

        [StringLength(15)] // Limits the length of the PhoneNumber
        public string PhoneNumber { get; set; }

        public bool IsActive { get; set; } = true; // Indicates whether the user account is active

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Default to the current time

        public DateTime? UpdatedAt { get; set; } // Nullable, as it may not be set at creation
                                                 // New property for roles
        public List<string> Roles { get; set; } = new List<string>(); // Assuming roles are stored as strings

    }
}
