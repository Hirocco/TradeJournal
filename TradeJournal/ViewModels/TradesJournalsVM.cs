using Microsoft.AspNetCore.Mvc;
using TradeJournal.Models;


namespace TradeJournal.ViewModels
{
    public class TradesJournalsVM
    {
        public Trade Trade { get; set; }
        public Journal Journal { get; set; }
    }
}
