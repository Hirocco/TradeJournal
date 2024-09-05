using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class RefreshToken : IEntityBase
    {
        public int Id { get; set; }
        public string TokenVal { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
        public DateTime RefreshTokenCreatedAt   { get; set; }

        public int AuthId { get; set; }
        public Auth Auth {  get; set; }
    }
}
