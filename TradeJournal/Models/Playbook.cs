using System.ComponentModel.DataAnnotations;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Playbook : IEntityBase
    {
        [Key]
        public int Id { get; set; } 

        public List<string> Conditions { get; set; } // warunki poszczegolnej strategii

        public int TradeId { get; set; }
        public Trade? Trade { get; set; }
    }
}
