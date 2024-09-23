using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Journal : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(max)")]
        public string Text { get; set; }
        
        [Column(TypeName = "varbinary(max)")]
        public byte[]? ByteStream { get; set; } = null;

        // relacje 
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

    }
}
