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
        public byte[]? Image { get; set; }

        [NotMapped]  // Ignorujemy to pole w bazie danych, potrzebne do przechowywania pliku w formularzu
        public IFormFile? ImageFile { get; set; }

        // relacje 
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

    }
}
