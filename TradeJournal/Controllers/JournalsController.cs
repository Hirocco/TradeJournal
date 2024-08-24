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


        // GET: Journals/AddOrEdit
        public IActionResult AddOrEdit(int id = 0)
        {
            Console.WriteLine("ŁADOWANIE");

            // Załaduj listę TradeId do ViewBag
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id");

            if (id == 0)
            {
                // Jeśli id jest 0, zwróć pusty model Journal do widoku
                return View(new Journal());
            }
            else
            {
                // Jeśli id jest różne od 0, znajdź istniejący wpis w bazie danych
                var journal = _context.Journals.FirstOrDefault(j => j.Id == id);
                if (journal == null)
                {
                    return NotFound();
                }
                return View(journal);
            }
        }

        // POST: Journals/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Text,TradeId")] Journal journal)
        {
            Console.WriteLine("BINDOWANIE");
            Console.WriteLine("IsValid: " + ModelState.IsValid);
            Console.WriteLine("Id: " + journal.Id);
            Console.WriteLine("Text: " + journal.Text);
            Console.WriteLine("TradeId: " + journal.TradeId);

            if (ModelState.IsValid)
            {
                Console.WriteLine("WALIDACJA PRZESZLA ");

                if (journal.Id == 0)
                {
                    // Dodaj nowy rekord
                    _context.Add(journal);
                }
                else
                {
                    // Aktualizuj istniejący rekord
                    _context.Update(journal);
                }

                // Zapisz zmiany w bazie danych
                await _context.SaveChangesAsync();

                // Przekieruj do Index lub innej strony po zapisaniu
                return RedirectToAction(nameof(Index));
            }

            // Jeśli model jest nieprawidłowy, zwróć użytkownika do widoku z błędami walidacji
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id", journal.TradeId);

            // Logowanie błędów walidacji do debugowania
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }

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
            if (_context.Journals == null) return Problem("Entity set 'ApplicationDbContext.Journals'  is null.");
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JournalExists(int id)
        {
            return _context.Journals.Any(e => e.Id == id);
        }
    }
}
