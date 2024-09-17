using System.ComponentModel.DataAnnotations;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class Playbook : IEntityBase
    {
        [Key]
        public int Id { get; set; } 

        public List<Condition> Conditions { get; set; } // warunki poszczegolnej strategii

        public int TradeId { get; set; }
        public Trade? Trade { get; set; }
    }

    public class Condition : IEntityBase
    {
        [Key]
        public int Id { get; set; }

        public string ConditionName { get; set; }
        public string ConditionId { get; set; }

        public int PlaybookId   { get; set; }
        public Playbook? Playbook { get; set; }
    
    }
}
