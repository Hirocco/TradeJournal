using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class User : IEntityBase
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public int AuthId {  get; set; }
        public Auth Auth { get; set; }

        [JsonIgnore]
        [IgnoreDataMember]
        public List<Trade> Trades { get; set; } = new List<Trade>();
    }
}
