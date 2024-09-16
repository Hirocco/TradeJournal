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
            if (playbook == null)
            {
                return NotFound();
            }

            return View(playbook);
        }

        // GET: Playbooks/Create
        public IActionResult Create()
        {
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id");
            return View();
        }

        // POST: Playbooks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Conditions,TradeId")] Playbook playbook)
        {
            if (ModelState.IsValid)
            {
                _context.Add(playbook);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id", playbook.TradeId);
            return View(playbook);
        }

        // GET: Playbooks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playbook = await _context.Playbooks.FindAsync(id);
            if (playbook == null)
            {
                return NotFound();
            }
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id", playbook.TradeId);
            return View(playbook);
        }

        // POST: Playbooks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Conditions,TradeId")] Playbook playbook)
        {
            if (id != playbook.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(playbook);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlaybookExists(playbook.Id))
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
            ViewData["TradeId"] = new SelectList(_context.Trades, "Id", "Id", playbook.TradeId);
            return View(playbook);
        }

        // GET: Playbooks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var playbook = await _context.Playbooks
                .Include(p => p.Trade)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (playbook == null)
            {
                return NotFound();
            }

            return View(playbook);
        }

        // POST: Playbooks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var playbook = await _context.Playbooks.FindAsync(id);
            if (playbook != null)
            {
                _context.Playbooks.Remove(playbook);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlaybookExists(int id)
        {
            return _context.Playbooks.Any(e => e.Id == id);
        }
    }
}
