using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeJournal.Data.Base;

namespace TradeJournal.Models
{
    public class TradingAccount : IEntityBase
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Login")]
        public string Login {  get; set; }
        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "Server name")]
        public string Server { get; set; }

        //RELACJA 
        List<Trade> trades { get; set; }

    }
}
