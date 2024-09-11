using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradeJournal.Data.DTOs;
using TradeJournal.Services.session;
using TradeJournal.Services.user;

namespace TradeJournal.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AuthApi : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;

        public AuthApi(IUserService userService, ISessionService sessionService)
        {
            _userService = userService;
            _sessionService = sessionService;
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

        [HttpPost("refreshToken")]
        public async Task RefreshToken([FromBody] string tokenVal)
        {
            try
            {
                await _userService.RefreshToken(tokenVal);
            }catch(Exception e) { throw new Exception(e.Message); }
        }
    }
}
