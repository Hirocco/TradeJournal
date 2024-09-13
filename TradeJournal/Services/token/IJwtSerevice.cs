using TradeJournal.Data.DTOs;
using TradeJournal.Models;

namespace TradeJournal.Services.token
{
    public interface IJwtTokenService
    {
        TokenDTO GenerateToken(User user);
    }
}
