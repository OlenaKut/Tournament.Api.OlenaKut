using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;

namespace Tournament.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;

        public AuthController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpPost]
        public async Task<ActionResult> RegisterUser(UserForRegistrationDto userForRegistrationDto)
        {
            var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegistrationDto);
            return result.Succeeded ? StatusCode(StatusCodes.Status201Created) : BadRequest(result.Errors);
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(UserForAuthenticationDto userForAuthenticationDto)
        {
            if (!await _serviceManager.AuthService.ValidateUserAsync(userForAuthenticationDto))
            {
                return Unauthorized("Invalid Authentication");
            }
            return Ok();
        }
    }
}
