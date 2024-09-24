using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Syncfusion.EJ2.Inputs;
using TradeJournal.Data;
using TradeJournal.Models;
using TradeJournal.ViewModels;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using TradeJournal.Migrations;

namespace TradeJournal.Controllers
{
    public class JournalsController : Controller
    {
        private readonly AppDbContext _context;
        public JournalsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Journals/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            
            var journal = await _context.Journals
                .Include(t => t.TradeId)
                .FirstOrDefaultAsync(t => t.Id == id);
            
            var trade = await _context.Trades.FirstOrDefaultAsync(t => t.Id == journal.TradeId);


            if (journal == null || trade == null)
            {
                return NotFound();
            }

            var viewModel = new TradesJournalsVM
            {
                Trade = trade,
                Journal = journal // Zakładamy, że Journal jest powiązany z Trade
            };

            return View("~/Views/Trades/Details.cshtml", viewModel);
        }

        // GET: Journals/AddOrEdit
        public IActionResult AddOrEdit(int tradeId, int id = 0)
        {
            if (id == 0) return View(new TradeJournal.Models.Journal { TradeId = tradeId });
            else
            {
                var journal = _context.Journals.FirstOrDefault(j => j.Id == id);
                if (journal == null) return NotFound();

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
                var existingJournal = await _context.Journals.FirstOrDefaultAsync(j => j.TradeId == journal.TradeId);

                if (existingJournal == null) { _context.Add(journal); } // nowy rekord

                else
                {
                    existingJournal.Text = journal.Text;
                   
                    _context.Update(existingJournal);
                }
                Console.WriteLine(journal.Id); Console.WriteLine(journal.TradeId);


                try { await _context.SaveChangesAsync(); } // Zapisz zmiany w bazie danych
                catch (DbUpdateConcurrencyException dbException)
                {
                    //The code from Microsoft - Resolving concurrency conflicts 
                    foreach (var entry in dbException.Entries)
                    {
                        if (entry.Entity is TradeJournal.Models.Journal)
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

            return Problem("Something went wrong");

        }

        private bool JournalExists(int id)
        {
            return _context.Journals.Any(e => e.Id == id);
        }
    }
}
