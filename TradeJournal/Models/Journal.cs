using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Journal : IEntityBase
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }

        public byte[] Image { get; set; }

        // relacje 
        public int TradeId { get; set; }
        public Trade Trade { get; set; }

    }
}
