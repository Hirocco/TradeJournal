using Microsoft.AspNetCore.Mvc;
using TradeJournal.Models;


namespace TradeJournal.ViewModels
{
    public class TradesJournalsPlaybooksVM
    {
        public Trade Trade { get; set; }
        public Journal Journal { get; set; }
        public Playbook Playbook { get; set; }  
    }
}
