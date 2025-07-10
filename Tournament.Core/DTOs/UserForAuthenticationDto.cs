using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.DTOs
{
    public record UserForAuthenticationDto
    {
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; init; } = null!;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; init; } = null!;
    }
}
