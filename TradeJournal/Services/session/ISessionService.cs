using TradeJournal.Data.DTOs;

namespace TradeJournal.Services.session
{
    public interface ISessionService
    {
        Task InitializeSessionAsync(int userId, string refreshToken);
        Task<string> GetUserIdAsync();
        Task<string> GetRefreshTokenAsync();
        Task<bool> IsSessionActiveAsync();
        Task RefreshSessionAsync(string refreshToken);
        Task ClearSessionAsync();
        Task CommitSession();
    }
}
