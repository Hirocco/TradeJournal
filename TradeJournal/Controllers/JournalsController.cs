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
using TradeJournal.ViewModels;

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
                .Include(t => t.TradeId)
                .FirstOrDefaultAsync(t => t.Id == id);

            var trade = await _context.Trades.FirstOrDefaultAsync(t => t.Id == journal.TradeId);


            if (journal == null || trade == null)
            {
                return NotFound();
            }

            var viewModel = new TradesJournalsViewModels
            {
                Trade = trade,
                Journal = journal // Zakładamy, że Journal jest powiązany z Trade
            };

            return View("~/Views/Trades/Details.cshtml", viewModel);
        }



        // GET: Journals/AddOrEdit
        public IActionResult AddOrEdit(int tradeId, int id = 0)
        {
            // blad jest dlatego, bo nie tworzy sie nowy journal, tylko kpiuje id z trade i probje aktualizowac cos co nie istnieje

            if (id == 0) return View(new Journal{ TradeId = tradeId }); // Jeśli id jest 0, zwróć pusty model Journal do widoku

            else
            {
                var journal = _context.Journals.FirstOrDefault(j => j.Id == id);
                if (journal == null) return NotFound(); // Jeśli id jest różne od 0, znajdź istniejący wpis w bazie danych

                return PartialView("AddOrEdit", journal);
            }
        }

        // POST: Journals/AddOrEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Text,TradeId")] Journal journal)
        {

            if (ModelState.IsValid)
            {
                var existingJounral = await _context.Journals.FirstOrDefaultAsync(j => j.TradeId == journal.TradeId);

                if (existingJounral == null) { _context.Add(journal); } // nowy rekord

                else 
                { 
                    existingJounral.Text = journal.Text;
                    _context.Update(existingJounral);
                }
                Console.WriteLine(journal.Id); Console.WriteLine(journal.TradeId);


                try { await _context.SaveChangesAsync(); } // Zapisz zmiany w bazie danych
                catch (DbUpdateConcurrencyException dbException)
                {
                    //The code from Microsoft - Resolving concurrency conflicts 
                    foreach (var entry in dbException.Entries)
                    {
                        if (entry.Entity is Journal)
                        {
                            var proposedValues = entry.CurrentValues; //Your proposed changes
                            var databaseValues = entry.GetDatabaseValues(); //Values in the Db

                            foreach (var property in proposedValues.Properties)
                            {
                                var proposedValue = proposedValues[property];
                                var databaseValue = databaseValues[property];

                                // TODO: decide which value should be written to database
                                // proposedValues[property] = <value to be saved>;
                            }

                            // Refresh original values to bypass next concurrency check
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                        else
                        {
                            throw new NotSupportedException(
                                "Don't know how to handle concurrency conflicts for "
                                + entry.Metadata.Name);
                        }
                    }
                }
                return RedirectToAction("Details", "Trades", new { id = journal.TradeId });
            }

            // Logowanie błędów walidacji do debugowania
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }

            return RedirectToAction("Details", "Trades");

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
