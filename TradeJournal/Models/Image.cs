using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Image : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string? ImageName { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Title { get; set; }


        [Column(TypeName = "nvarchar(max)")]
        public string? FileContent { get; set; }

        [NotMapped]
        public IFormFile ImageFile{ get; set; }

        //relacje
        public int TradeId { get; set; }
        public Trade? Trade { get; set; }

    }

}
