using Microsoft.CodeAnalysis.Scripting;
using System.Threading.Tasks;
using TradeJournal.Data.DTOs;
using TradeJournal.Services.token;
using BCrypt;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using TradeJournal.Data;
using TradeJournal.Models;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Migrations;



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

            var tokenDto = _jwtTokenService.GenerateToken(newUser);
            var token = new RefreshToken
            {
                TokenVal = tokenDto.Ref_Token,
                RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(1),    // REFRESH TOKEN CZAS 1H - Zrobie setup pozniej
                AuthId = newUser.Auth.Id,
            };
            await _context.Tokens.AddAsync(token);
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
            // Podłączamy token do auth, auth do usera i sprawdzamy wartość tokenu
            var token = await _context.Tokens.Include(t => t.Auth).ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.TokenVal == refreshToken);

            if (token == null || token.RefreshTokenExpiresAt <= DateTime.UtcNow)
                throw new UnauthorizedAccessException("Refresh token expired");

            var user = token.Auth.User; // Używamy użytkownika, który już został załadowany w poprzednim zapytaniu
            if (user == null)
                throw new Exception("User not found - user service refresh token");

            // Generujemy nowe dane do tokenu
            var newTokenDto = _jwtTokenService.GenerateToken(user);

            // Przypisujemy do istniejącego tokenu
            token.TokenVal = newTokenDto.Ref_Token;
            token.RefreshTokenExpiresAt = DateTime.UtcNow.AddHours(1);

            // Aktualizujemy token w bazie
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();

            return newTokenDto;
        }


    }
}
