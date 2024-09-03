using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;
using TradeJournal.ViewModels;

namespace TradeJournal.Controllers
{
    public class TradesController : Controller
    {
        private readonly AppDbContext _context;
        public TradesController(AppDbContext context)
        {
            _context = context;

        }

        // GET: Trades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trades.ToListAsync());
        }


        // GET: Trades/Details/{Id}
        public async Task<IActionResult> Details(int? id)
        {
            // nie znaleziono/istnieje id 
            if (id == null) return NotFound();
            

            var trade = await _context.Trades
                .FirstOrDefaultAsync(m => m.Id == id);

            //nie znaleziono trade
            if (trade == null) return NotFound();
     
            //podlaczanie notatki pod trade
            var journal = await _context.Journals.FirstOrDefaultAsync(j=>j.TradeId == trade.Id);
            if (journal == null)
            {
                journal = new Journal
                {
                    TradeId = trade.Id,  
                    Trade = trade        
                };
            }

            //tworzenie viewModelu
            var viewModel = new TradesJournalsViewModels
            {
                Trade = trade,
                Journal = journal
            };

            
           //zwracamy oba
            return View(viewModel);
        }

    

        // GET: Trades/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            // odpala sie 
            if (id == 0) { return View(new Trade());  }
            else return View(_context.Trades.Find(id));
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,TransactionOpenDate,TransactionCloseDate,SymbolName,PositionType,PositionVolume,EntryPrice,StopLoss,TakeProfit,Comission,Swap,TradeOutcome,PriceChange")] Trade trade)
        {

            if (ModelState.IsValid) 
            {
                if(trade.Id == 0) _context.Add(trade);
                else _context.Update(trade);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)  System.Diagnostics.Debug.WriteLine(error.ErrorMessage);

            return View(trade);
        }


        // POST: Trades/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trade = await _context.Trades.FindAsync(id);
            if (trade != null)
            {
                _context.Trades.Remove(trade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TradeExists(int id)
        {
            return _context.Trades.Any(e => e.Id == id);
        }
        
        


    }
}
