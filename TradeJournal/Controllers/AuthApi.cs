using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeJournal.Data.DTOs;
using TradeJournal.Services.user;

namespace TradeJournal.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthApi : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthApi(IUserService userService)
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
            Console.WriteLine($"Register: {userRegisterDto}");
            try
            {
                await _userService.RegisterUserAsync(userRegisterDto);
                return Ok();
            }
            catch (Exception e) { return Problem(e.Message); }
        }

        // ???
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] string refreshToken)
        {
            Console.WriteLine("ODPALA");
            try
            {
                
                var tokenDto = await _userService.RefreshTokenAsync(refreshToken);
                Console.WriteLine($"param: {refreshToken}   -   tokenDto: {tokenDto.Ref_Token}");
                return Ok(tokenDto);
            }
            catch(UnauthorizedAccessException e) { return Unauthorized(e.Message); }
        }
    }
}
