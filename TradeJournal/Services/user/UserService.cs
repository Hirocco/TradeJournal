using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;
using TradeJournal.Data.DTOs;
using TradeJournal.Services.token;
using BCrypt;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TradeJournal.Data;
using TradeJournal.Models;
using Microsoft.EntityFrameworkCore;



namespace TradeJournal.Services.user
{
    public class UserService : IUserService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly AppDbContext _context;

        public UserService(IJwtTokenService jwtTokenService, AppDbContext context)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
        }

        // login - register - refresh token 

        public async Task RegisterUserAsync(UserRegisterDTO userRegisterDto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

            var newUser = new User
            {
                Login = userRegisterDto.Login,
                Auth = new Auth
                {
                    Email = userRegisterDto.Email,
                    Password = hashedPassword
                }
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();  
            
        }

        public async Task<TokenDTO> LoginAsync(AuthDTO authDto)
        {
            var auth = await _context.Auths.FirstOrDefaultAsync(a => a.Email == authDto.Email);
            if (auth == null || !BCrypt.Net.BCrypt.Verify(authDto.Password, auth.Password)) throw new UnauthorizedAccessException("Invalid login or password!");

            var user = await _context.Users.Include(u => u.Auth).FirstOrDefaultAsync(u => u.Auth.Email == authDto.Email);
            if (user == null) throw new Exception("User doesn't exist");
            
            var tokenDto = _jwtTokenService.GenerateToken(user);
            var token = new RefreshToken
            {
                TokenVal = tokenDto.Ref_Token,
                RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(1),    // REFRESH TOKEN CZAS 1H - Zrobie setup pozniej
                AuthId = auth.Id,
            };

            await _context.Tokens.AddAsync(token);

            return tokenDto;

        }
        public async Task<TokenDTO> RefreshTokenAsync(string refreshToken)
        {
            // podlaczamy token do auth, auth do usera i sprawdzamy wartosc tokenu
            var token = await _context.Tokens.Include(t => t.Auth).ThenInclude(t => t.User).FirstOrDefaultAsync(t => t.TokenVal == refreshToken);

            if (token == null || token.RefreshTokenExpiresAt <= DateTime.UtcNow) throw new UnauthorizedAccessException("Refresh token expired");

            // podlaczam usera do auth, auth do tokenu i sprawdzam czy token nalezy do usera
            var user = await _context.Users.Include(u => u.Auth).ThenInclude(u => u.RefreshToken).FirstOrDefaultAsync(u => u.Id == token.Auth.UserId);
            if (user == null) throw new Exception("User not found - user service refresh token");
            
            // generujemy nowe dane do tokenu
            var newTokenDto = _jwtTokenService.GenerateToken(user);

            //przypisujemy do istniejacego tokenu
            token.TokenVal = newTokenDto.Ref_Token;
            token.RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(1);
            
            //idzie update do bazy
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();

            return newTokenDto;
        }

    }
}
