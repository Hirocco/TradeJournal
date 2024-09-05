using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeJournal.Data.DTOs;
using TradeJournal.Services.user;

namespace TradeJournal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthApi : ControllerBase
    {
        private readonly UserService _userService;

        public AuthApi(UserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AuthDTO authDto)
        {
            try
            {
                var tokenDto = await _userService.LoginAsync(authDto);
                return Ok(tokenDto);
            }
            catch(UnauthorizedAccessException e) { return Unauthorized(e.Message); }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterDTO userRegisterDto)
        {
            try
            {
                await _userService.RegisterUserAsync(userRegisterDto);
                return Ok();
            }
            catch (Exception e) { return Problem(e.Message); }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            try
            {
                var tokenDto = await _userService.RefreshTokenAsync(refreshToken);
                return Ok(tokenDto);
            }
            catch(UnauthorizedAccessException e) { return Unauthorized(e.Message); }
        }

    }
}
