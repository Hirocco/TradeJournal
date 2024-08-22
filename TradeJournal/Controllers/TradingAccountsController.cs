using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;

namespace TradeJournal.Controllers
{
    public class TradingAccountsController : Controller
    {
        private readonly AppDbContext _context;

        public TradingAccountsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: TradingAccounts
        public async Task<IActionResult> Index()
        {
            return View(await _context.TradingAccounts.ToListAsync());
        }

        // GET: TradingAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tradingAccount = await _context.TradingAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tradingAccount == null)
            {
                return NotFound();
            }

            return View(tradingAccount);
        }

        // GET: TradingAccounts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TradingAccounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Login,Password,Server")] TradingAccount tradingAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tradingAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tradingAccount);
        }

        // GET: TradingAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tradingAccount = await _context.TradingAccounts.FindAsync(id);
            if (tradingAccount == null)
            {
                return NotFound();
            }
            return View(tradingAccount);
        }

        // POST: TradingAccounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Login,Password,Server")] TradingAccount tradingAccount)
        {
            if (id != tradingAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tradingAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TradingAccountExists(tradingAccount.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tradingAccount);
        }

        // GET: TradingAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tradingAccount = await _context.TradingAccounts
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tradingAccount == null)
            {
                return NotFound();
            }

            return View(tradingAccount);
        }

        // POST: TradingAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tradingAccount = await _context.TradingAccounts.FindAsync(id);
            if (tradingAccount != null)
            {
                _context.TradingAccounts.Remove(tradingAccount);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TradingAccountExists(int id)
        {
            return _context.TradingAccounts.Any(e => e.Id == id);
        }
    }
}
