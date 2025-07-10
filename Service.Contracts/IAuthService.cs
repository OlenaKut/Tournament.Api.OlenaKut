using Microsoft.AspNetCore.Identity;
using Tournament.Core.DTOs;

namespace Services.Contracts
{
    public interface IAuthService
    {
        Task<TokenDto> CreateTokenAsync(bool expireTime);
        Task<TokenDto> RefreshTokenAsync(TokenDto token);
        Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto);
        Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto);
    }
}