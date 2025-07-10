using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public AuthService(IMapper mapper, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
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

    }
}
