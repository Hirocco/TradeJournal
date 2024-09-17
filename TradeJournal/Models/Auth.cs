using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Auth : IEntityBase
    {
        public int Id { get; set; }

        public string Email { get; set; }
        public string Password { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }
 
        public List<RefreshToken> RefreshToken { get; set; }

    }
}
