using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;

namespace TradeJournal.Controllers
{
    public class JournalsController : Controller
    {
        private readonly AppDbContext _context;

        public JournalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Journals
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Journals.Include(j => j.Trade);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Journals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .Include(j => j.Trade)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }


        // GET: Journals/Create
        public IActionResult AddOrEdit(int id=0)
        {
            Console.WriteLine("ŁADOWANIE");
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id");
			if (id == 0) return View(new Journal());
			else return View(_context.Journals.Find(id));
		}

        // POST: Journals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Text,TradeId")] Journal journal)
        {
            Console.WriteLine("BINDOWANIE");
            // DO TEGO MOMENTU DZIALA
            if (ModelState.IsValid)
            {
				Console.WriteLine("WALIDACJA PRZESZLA ");

                if (journal.Id == 0) _context.Add(journal);
                else _context.Update(journal);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id", journal.TradeId);

              var errors = ModelState.Values.SelectMany(v => v.Errors);
              foreach (var error in errors)  System.Diagnostics.Debug.WriteLine(error.ErrorMessage);

            return View(journal);
        }

        // GET: Journals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var journal = await _context.Journals
                .Include(j => j.Trade)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (journal == null)
            {
                return NotFound();
            }

            return View(journal);
        }

        // POST: Journals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var journal = await _context.Journals.FindAsync(id);
            if (journal != null)
            {
                _context.Journals.Remove(journal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalExists(int id)
        {
            return _context.Journals.Any(e => e.Id == id);
        }
    }
}
