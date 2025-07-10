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
    public class TokenController : ControllerBase
    {
        private readonly IAuthService _authService;
        public TokenController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult> RefreshToken(TokenDto token)
        {
            TokenDto tokenDto = await _authService.RefreshTokenAsync(token);
            return Ok(tokenDto);
        }
    }
}
