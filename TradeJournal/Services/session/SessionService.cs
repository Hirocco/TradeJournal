using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using TradeJournal.Data;
using TradeJournal.Data.DTOs;
using System.Text.Json;
using System.Web;


namespace TradeJournal.Services.session
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        // Inicjalizacja sesji użytkownika po zalogowaniu - mozliwe ze json trzeba 
        public Task InitializeSessionAsync(int userId, string refreshToken)
        {
            _httpContextAccessor.HttpContext.Session.LoadAsync();
            _httpContextAccessor.HttpContext.Session.SetString("UserId", JsonSerializer.Serialize(userId));
            _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", JsonSerializer.Serialize(refreshToken));
            this.CommitSession();

            Console.WriteLine($"user id: {_httpContextAccessor.HttpContext.Session?.GetString("UserId")}");
            Console.WriteLine($"Refresh token: {_httpContextAccessor.HttpContext.Session?.GetString("RefreshToken")}");

            return Task.CompletedTask;
        }

        // Pobiera UserId z sesji
        public Task<string> GetUserIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if (userId == null) throw new Exception("Session could not get user");
     
            return Task.FromResult(JsonSerializer.Deserialize<string>(userId));
        }


        // Pobiera RefreshToken z sesji
        public Task<string> GetRefreshTokenAsync()
        {
            var reftoken = _httpContextAccessor.HttpContext.Session.GetString("RefreshToken");

            // Jeśli session jest pusta, zwróć null
            if (string.IsNullOrEmpty(reftoken))
            {
                return Task.FromResult<string>(null);
            }
            return Task.FromResult(JsonSerializer.Deserialize<string>(_httpContextAccessor.HttpContext.Session.GetString("RefreshToken")));
        }

        // Sprawdza, czy sesja użytkownika jest aktywna
        public Task<bool> IsSessionActiveAsync()
        {
            var userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            return Task.FromResult(JsonSerializer.Deserialize<bool>(!string.IsNullOrEmpty(userId)));
        }

        // Odświeżenie sesji
        public Task RefreshSessionAsync(string refreshToken)
        {
            _httpContextAccessor.HttpContext.Session.SetString("RefreshToken", refreshToken);
            this.CommitSession();
            return Task.CompletedTask;
        }

        // Wyczyszczenie sesji
        public Task ClearSessionAsync()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
            return Task.CompletedTask;
        }
        
        public async Task CommitSession()
        {
            _httpContextAccessor.HttpContext.Session.CommitAsync();
        }

    }
}
