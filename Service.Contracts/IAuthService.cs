using Microsoft.AspNetCore.Identity;
using Tournament.Core.DTOs;

namespace Services.Contracts
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto);
        Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto);
    }
}