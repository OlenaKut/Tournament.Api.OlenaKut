using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;
using Tournament.Core.Entities;

namespace Tournament.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _config;
        private ApplicationUser? user;

        public AuthService(IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration config)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
        }

        public async Task<TokenDto> CreateTokenAsync(bool expireTime)
        {
            ArgumentNullException.ThrowIfNull(nameof(user));
            SigningCredentials signing = GetSigningCredentials();
            IEnumerable<Claim> claims = await GetClaimsAsync();
            JwtSecurityToken tokenOptions = GenerateTokenOptions(signing, claims);

            //return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            user!.RefreshToken = GenerateRefreshToken();

            if (expireTime)
            {
                user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            }

            var res = await _userManager.UpdateAsync(user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            return new TokenDto(accessToken, user.RefreshToken!);

        }

        private string? GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signing, IEnumerable<Claim> claims)
        {
            var jwtSettings = _config.GetSection("JwtSettings");

            var tokenOptions = new JwtSecurityToken(
                    issuer: jwtSettings["Issuer"],
                    audience: jwtSettings["Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["Expires"])),
                    signingCredentials: signing
                );
            return tokenOptions;
        }

        private async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            ArgumentNullException.ThrowIfNull(nameof(user));

            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
            };

            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;

        }

        private SigningCredentials GetSigningCredentials()
        {
            var secretKey = _config["secretkey"];
            ArgumentNullException.ThrowIfNull(secretKey, nameof(secretKey));
            byte[] key = Encoding.UTF8.GetBytes(secretKey);
            var secret = new SymmetricSecurityKey(key);

            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }




        public async Task<IdentityResult> RegisterUserAsync(UserForRegistrationDto userForRegistrationDto)
        {
            ArgumentNullException.ThrowIfNull(nameof(userForRegistrationDto));

            var roleExists = await _roleManager.RoleExistsAsync(userForRegistrationDto.Role);
            if (!roleExists)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Role does not exist" });
            }

            var user = _mapper.Map<ApplicationUser>(userForRegistrationDto);

            var result = await _userManager.CreateAsync(user, userForRegistrationDto.Password!);

            if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, userForRegistrationDto.Role);
                }
            return result;
        }

        public async Task<bool> ValidateUserAsync(UserForAuthenticationDto userForAuthenticationDto)
        {
            if (userForAuthenticationDto == null)
            {
                throw new ArgumentNullException(nameof(userForAuthenticationDto));
            }

            user = await _userManager.FindByEmailAsync(userForAuthenticationDto.Email);
            return user != null && await _userManager.CheckPasswordAsync(user, userForAuthenticationDto.Password);
        }


        public async Task<TokenDto> RefreshTokenAsync(TokenDto token)
        {
            ClaimsPrincipal principal = GetPrincipalFromExpiredToken(token.AccessToken);

            ApplicationUser? user = await _userManager.FindByNameAsync(principal.Identity?.Name!);

            if (user == null || user.RefreshToken != token.RefreshToken || user.RefreshTokenExpireTime <= DateTime.UtcNow)
            {
                throw new ArgumentException("The TokenDto has some invalid values");
            }

            this.user = user;

            return await CreateTokenAsync(expireTime: false);
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var jwtSettings = _config.GetSection("JwtSettings");
            ArgumentNullException.ThrowIfNull(nameof(jwtSettings));

            var secretKey = _config["secretkey"];
            ArgumentNullException.ThrowIfNull(nameof(secretKey));

            var tokenValidationParams = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false // Ändras till false
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            ClaimsPrincipal principal = tokenHandler.ValidateToken(accessToken, tokenValidationParams, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }

            return principal;

        }

    }
}
