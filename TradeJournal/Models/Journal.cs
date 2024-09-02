using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Journal : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(1000)")]

        public string Text { get; set; }

        // relacje 
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

    }
}
