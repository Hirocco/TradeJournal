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


        /*hashowanie -> tworzenie obiektu user -> dodanie do BD*/
        /*Generowanie tokenu -> tworzenie obiektu RefreshToken -> dodanie do BD*/
        public async Task RegisterUserAsync(UserRegisterDTO userRegisterDto)
        {

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userRegisterDto.Password);

            var newUser = new User
            {
                Login = userRegisterDto.Login,
                Auth = new Auth
                {
                    Email = userRegisterDto.Email,
                    Password = hashedPassword,
                },
            };

            // szybka walidacja
            var email = await _context.Users.FirstOrDefaultAsync(u => u.Auth.Email == newUser.Auth.Email);
            if (email != null) throw new Exception("Email already in use!");

            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();

            // tutaj token jest taki sam jak w BD
            var tokenDto = _jwtTokenService.GenerateToken(newUser);
            var token = new RefreshToken
            {
                TokenVal = tokenDto.Ref_Token,
                RefreshTokenExpiresAt = DateTime.Now.AddMinutes(2),    // REFRESH TOKEN CZAS 1H - Zrobie setup pozniej
                AuthId = newUser.Auth.Id,
            };
            Console.WriteLine($"Register user token: {token.TokenVal}");
            
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();

        }

        public async Task<TokenDTO> LoginAsync(AuthDTO authDto)
        {
            var auth = await _context.Auths.FirstOrDefaultAsync(a => a.Email == authDto.Email);
            if (auth == null || !BCrypt.Net.BCrypt.Verify(authDto.Password, auth.Password)) throw new UnauthorizedAccessException("Invalid login or password!");

            var user = await _context.Users.Include(u => u.Auth).FirstOrDefaultAsync(u => u.Auth.Email == authDto.Email);
            if (user == null) throw new Exception("User doesn't exist");

            var userToken = await _context.Tokens.FirstOrDefaultAsync(t => t.AuthId == auth.Id && auth.Email == user.Auth.Email);
            Console.WriteLine($"userToken: {userToken.TokenVal} {userToken.RefreshTokenExpiresAt} - przed utworzeniem nowego do sesji");
            Console.WriteLine(DateTime.Now);

            // Jeżeli token istnieje i jest nadal ważny to znajdź go i aktualizuj
            if (userToken != null && userToken.RefreshTokenExpiresAt > DateTime.Now)
            {

                // Sprawdź, czy token jest już śledzony przez kontekst EF Core
                var existingToken = await _context.Tokens
                    .AsTracking() // Śledzenie tokenu, aby upewnić się, że aktualizacje są zapisywane
                    .FirstOrDefaultAsync(t => t.TokenVal == userToken.TokenVal);

                if (existingToken != null)
                {
                    // Zaktualizuj właściwości istniejącego tokenu
                    existingToken.RefreshTokenExpiresAt = DateTime.Now.AddMinutes(2);  // przedłużenie o godzinę

                    // Wygeneruj nowy token dostępu (access token)
                    var updatedAccessToken = _jwtTokenService.GenerateToken(user);
                    updatedAccessToken.Ref_Token = existingToken.TokenVal; // Pozostaw ten sam RefreshToken

                    // Zapisz zmiany w bazie danych bez wywoływania `Update()`
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Acess token: {updatedAccessToken.A_Token}");
                    return updatedAccessToken;
                }
            }

            /*Jeżeli token wygasł dodaj nowy*/
            var existingToken2 = await _context.Tokens
                .AsTracking() 
                .FirstOrDefaultAsync(t => t.TokenVal == userToken.TokenVal);

            if (existingToken2 == null) throw new Exception("Token not assigned, have you registered? ");

            existingToken2.RefreshTokenExpiresAt = DateTime.Now.AddMinutes(2);
            var tokenDto = _jwtTokenService.GenerateToken(user);

            existingToken2.TokenVal = tokenDto.Ref_Token;
            await _context.SaveChangesAsync();
            Console.WriteLine($"Login token: {existingToken2.TokenVal}");

            return tokenDto;
        }
        public async Task<TokenDTO> RefreshTokenAsync(string refreshToken)
        {
            // Podłączamy token do auth, auth do usera i sprawdzamy wartość tokenu
            var token = await _context.Tokens.Include(t => t.Auth).ThenInclude(a => a.User)
                .FirstOrDefaultAsync(t => t.TokenVal == refreshToken);

            if (token == null || token.RefreshTokenExpiresAt <= DateTime.Now)
                throw new UnauthorizedAccessException("Refresh token expired");

            var user = token.Auth.User; // Używamy użytkownika, który już został załadowany w poprzednim zapytaniu
            if (user == null)
                throw new Exception("User not found - user service refresh token");

            // Generujemy nowe dane do tokenu
            var newTokenDto = _jwtTokenService.GenerateToken(user);

            // Przypisujemy do istniejącego tokenu
            token.TokenVal = newTokenDto.Ref_Token;
            token.RefreshTokenExpiresAt = DateTime.Now.AddHours(1);

            // Aktualizujemy token w bazie
            _context.Tokens.Update(token);
            await _context.SaveChangesAsync();

            return newTokenDto;
        }


    }
}
