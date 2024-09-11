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
using Microsoft.AspNetCore.Mvc;
using TradeJournal.Services.session;



namespace TradeJournal.Services.user
{
    public class UserService : IUserService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ISessionService _sessionService;
        private readonly AppDbContext _context;

        public UserService(IJwtTokenService jwtTokenService, AppDbContext context, ISessionService sessionService)
        {
            _context = context;
            _jwtTokenService = jwtTokenService;
            _sessionService = sessionService;
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
                    await _sessionService.InitializeSessionAsync(user.Id, updatedAccessToken.Ref_Token);



                    Console.WriteLine($"Acess token: {updatedAccessToken.A_Token}");
                    return updatedAccessToken;
                }
            }

            /*Jeżeli token wygasł dodaj nowy*/
            
            var tokenDto = _jwtTokenService.GenerateToken(user);

            var newToken = new RefreshToken
            {
                TokenVal = tokenDto.Ref_Token,
                AuthId = auth.Id,
                RefreshTokenExpiresAt = DateTime.Now.AddMinutes(2)
            };
            
            _context.Tokens.Add(newToken);
            await _context.SaveChangesAsync();
            await _sessionService.InitializeSessionAsync(user.Id, tokenDto.Ref_Token);
            Console.WriteLine($"Sesja aktywna: {await _sessionService.IsSessionActiveAsync()}");

            Console.WriteLine($"Login token: {newToken.TokenVal}");

            return tokenDto;
        }

        public async Task RefreshToken(string refTokenVal)
        {
            var currUser = await _context.Users
                .Include(u => u.Auth)
                .ThenInclude(a => a.RefreshToken)
                .FirstOrDefaultAsync(u => u.Auth.RefreshToken.Any(rt => rt.TokenVal == refTokenVal));


            if (currUser == null) throw new Exception("User not found - refresh token exeption");

            var auth = await _context.Auths.FirstOrDefaultAsync(a => a.UserId == currUser.Id);
            if (auth == null) throw new Exception("Auth for user not found - refresh token exeption");

            var userToken = await _context.Tokens.FirstOrDefaultAsync(t => t.AuthId == auth.Id);
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
                    var updatedAccessToken = _jwtTokenService.GenerateToken(currUser);
                    updatedAccessToken.Ref_Token = existingToken.TokenVal; // Pozostaw ten sam RefreshToken

                    // Zapisz zmiany w bazie danych bez wywoływania `Update()`
                    await _context.SaveChangesAsync();

                    Console.WriteLine($"Acess token: {updatedAccessToken.A_Token}");
                }
            }
            else throw new Exception("Token expired");

        }

        public async Task<UserDTO> GetCurrentUser()
        {
            var userId = await _sessionService.GetUserIdAsync();
            var currentUser = await _context.Users.FindAsync(userId);
            return new UserDTO
            {
                Id = currentUser.Id,
                Login = currentUser.Login,
            };
        }
        

    }
}
