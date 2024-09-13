using TradeJournal.Data.DTOs;

namespace TradeJournal.Services.user
{
    public interface IUserService
    {
        Task<TokenDTO> LoginAsync(AuthDTO authDto);
        Task RegisterUserAsync(UserRegisterDTO userRegisterDto);
        Task RefreshToken(string refTokenVal);

        Task<UserDTO> GetCurrentUser();

        Task Logout();
    }
}
