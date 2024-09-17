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
using TradeJournal.Services.user;
using TradeJournal.ViewModels;

namespace TradeJournal.Controllers
{
    public class PlaybooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IUserService _userService;

        public PlaybooksController(AppDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<IActionResult> Index(int tradeId)
        {
            var playbook = await _context.Playbooks
                .Include(p => p.Conditions)
                .FirstOrDefaultAsync(p => p.TradeId == tradeId);

            if (playbook == null)
            {
                playbook = new Playbook
                {
                    TradeId = tradeId,
                    Conditions = new List<Condition>()
                };
                _context.Playbooks.Add(playbook);
                await _context.SaveChangesAsync();
            }

            return PartialView("Index", playbook);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateCondition(int playbookId, string conditionName)
        {
            var playbook = await _context.Playbooks
                .Include(p => p.Conditions)
                .FirstOrDefaultAsync(p => p.Id == playbookId);

            if (playbook == null) return NotFound();

            var condition = new Condition
            {
                ConditionName = conditionName,
                PlaybookId = playbook.Id
            };

            _context.Conditions.Add(condition);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { tradeId = playbook.TradeId });
        }

        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteCondition(int conditionId)
        {
            var condition = await _context.Conditions.FindAsync(conditionId);
            if (condition == null) return NotFound();

            _context.Conditions.Remove(condition);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", new { tradeId = condition.Playbook.TradeId });
        }
    }

}
