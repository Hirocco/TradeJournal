using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Migrations;
using TradeJournal.Models;
using TradeJournal.ViewModels;

namespace TradeJournal.Controllers
{
    public class PlaybooksController : Controller
    {
        private readonly AppDbContext _context;

        public PlaybooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Playbooks
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Playbooks.Include(p => p.Trade);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Playbooks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playbook = await _context.Playbooks
                .Include(p => p.Trade)
                .FirstOrDefaultAsync(m => m.Id == id);

            var trade = await _context.Trades.FirstOrDefaultAsync(t => t.Id == playbook.TradeId);


            if (playbook == null || trade == null)
            {
                return NotFound();
            }

            var viewModel = new TradesJournalsPlaybooksVM
            {
                Trade = trade,
                Playbook = playbook
            };

            return View("~/Views/Trades/Details.cshtml", viewModel);
        }

        // GET: Playbooks/AddOrEdit
        public IActionResult AddOrEdit(int tradeId,int id = 0)
        {
            if (id == 0) return View(new Playbook { TradeId = tradeId });
            else
            {
                var playbook = _context.Playbooks.FirstOrDefault(p => p.Id == id);
                if (playbook == null) return NotFound();

                return PartialView("AddOrEditPlaybook", playbook);
            }
        }

        //post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("Id,Conditions,TradeId")] Playbook playbook)
        {
            if (ModelState.IsValid)
            {
                var existingPlaybook = await _context.Playbooks.FirstOrDefaultAsync(p => p.TradeId == playbook.TradeId);
                if (existingPlaybook == null) _context.Add(playbook);
                else
                {
                    existingPlaybook.Conditions = playbook.Conditions;
                    _context.Update(existingPlaybook);
                }
                try { _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException dbException)
                {
                    //The code from Microsoft - Resolving concurrency conflicts 
                    foreach (var entry in dbException.Entries)
                    {
                        if (entry.Entity is Playbook)
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
            }
            // Logowanie błędów walidacji do debugowania
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                System.Diagnostics.Debug.WriteLine(error.ErrorMessage);
            }

            return RedirectToAction("Details", "Trades");
        }

      

        private bool PlaybookExists(int id)
        {
            return _context.Playbooks.Any(e => e.Id == id);
        }
    }
}
