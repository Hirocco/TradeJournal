using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;
using TradeJournal.Services.session;
using TradeJournal.Services.user;
using TradeJournal.ViewModels;

namespace TradeJournal.Controllers
{
    public class TradesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;
        private readonly ISessionService _sessionService;
        public TradesController(AppDbContext context, IUserService userService, ISessionService sessionService)
        {
            _context = context;
            _userService = userService;
            _sessionService = sessionService;
        }

        // GET: Trades
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userService.GetCurrentUser();
            var trades = await _context.Trades
                .Where(t => t.UserId == currentUser.Id) // Filtruj tylko transakcje zalogowanego użytkownika
                .ToListAsync();

            return View(trades);
        }


        // GET: Trades/Details/{Id}
        public async Task<IActionResult> Details(int? id)
        {
            
            var currentUser = await _userService.GetCurrentUser();
            // nie znaleziono/istnieje id 
            if (id == null) return NotFound();


            var trade = await _context.Trades
               .FirstOrDefaultAsync(m => m.Id == id && m.UserId == currentUser.Id); // Upewnij się, że transakcja należy do zalogowanego użytkownika

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
            if (id == 0) return View(new Trade());
            else return View(_context.Trades.Find(id));
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,UserId,TransactionOpenDate,TransactionCloseDate,SymbolName,PositionType,PositionVolume,EntryPrice,StopLoss,TakeProfit,Comission,Swap,TradeOutcome,PriceChange")] Trade trade)
        {
            Console.WriteLine($"Sesja aktywna: {await _sessionService.IsSessionActiveAsync()}");
            HttpContext.Session.TryGetValue("key", out var key);
            var currentUser = await _userService.GetCurrentUser();

            if (ModelState.IsValid) 
            {
                if(trade.Id == 0) 
                { 
                    trade.UserId = currentUser.Id;
                    _context.Add(trade); 
                }
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
            var currentUser = await _userService.GetCurrentUser();

            if (trade != null && trade.UserId == currentUser.Id)
            {
                _context.Trades.Remove(trade);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
