using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record UserForRegistrationDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; init; } = null!;

        [Required(ErrorMessage = "Username is required")]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        public string UserName { get; init; } = null!;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(3, ErrorMessage = "Password must be at least 3 characters.")]
        public string Password { get; init; } = null!;

        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; init; } = null!;

        [Required(ErrorMessage = "FirstName is required")]
        public string FirstName { get; init; } = null!;

        [Required(ErrorMessage = "LastName is required")]
        public string LastName { get; init; } = null!;

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; init; } = null!;
    }
}
