using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;
using CsvHelper.Configuration;
using CsvHelper;

namespace TradeJournal.Controllers
{
    public class TradesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ImportTrade _csvImporter;

        public TradesController(AppDbContext context)
        {
            _context = context;
            _csvImporter = new ImportTrade(context);

        }

        // GET: Trades
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trades.ToListAsync());
        }

        // GET: Trades/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                
                return NotFound();
            }

            var trade = await _context.Trades
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trade == null)
            {
                return NotFound();
            }

            return View(trade);
        }

        /*Nie testowane*/
        public IActionResult ImportCSV(string filePath)
        {
            try
            {
                if (System.IO.File.Exists(filePath))
                {

                    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };
                    using (StreamReader streamReader = new StreamReader(filePath))
                    using (CsvReader csvReader = new CsvReader(streamReader, config))
                    {

                        // Read records from the CSV file
                        IEnumerable<Trade> records = csvReader.GetRecords<Trade>();

                        // Process each record
                        foreach (Trade trade in records)
                        {
                            Console.WriteLine($"Id: {trade.Id}, Symbol: {trade.SymbolName}");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
            return View();
        }

       

        // GET: Trades/AddOrEdit
        public IActionResult AddOrEdit(int id=0)
        {
            // odpala sie 
            if (id == 0) return View(new Trade());
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
        


        /*TODO NAPRAWIC MAPOWANIE IDK CZEMU NIE ZNAJDUJE WIDOKU*/
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> ImportTrade(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var filePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(filePath))
                {
                    await file.CopyToAsync(stream);
                }

                await _csvImporter.ImportCsvAsync(filePath);
            }

            return View();
        }

    }
}
