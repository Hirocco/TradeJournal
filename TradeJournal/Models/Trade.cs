﻿using System.ComponentModel.DataAnnotations;
using TradeJournal.Data.Base;
using TradeJournal.Data;

namespace TradeJournal.Models
{
    public class Trade : IEntityBase
    { 
        [Key]   
        public int Id { get; set; }

        public DateTime TransactionOpenDate { get; set; }
        public DateTime TransactionCloseDate { get; set; }
        public DateTime TradeAdded { get; set; } = DateTime.Now;
        public string SymbolName { get; set; }
        public string PositionType { get; set; }
        public float PositionVolume { get; set; }
        public float EntryPrice { get; set; }
        public float StopLoss { get; set; }
        public float TakeProfit { get; set; }
        public float Comission {  get; set; }
        public float Swap { get; set; }
        public float TradeOutcome { get; set; } // P/L
        public float PriceChange { get; set; } // (%)

        //Relacja 
        //public ICollection<Journal> ?Journals { get;} // chyba nie trzeba 
    }
}
