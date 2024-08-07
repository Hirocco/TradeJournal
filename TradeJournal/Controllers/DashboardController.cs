using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using TradeJournal.Data;
using TradeJournal.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace TradeJournal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _appDbContext;
        public DashboardController(AppDbContext context)
        {
            _appDbContext = context;
        }
        public async Task<ActionResult> Index()
        {


            // wszystkie trade
            List<Trade> selectedTrades = await _appDbContext.Trades.ToListAsync();

            // total swaps + commisions 
            float totalSwapComissions = selectedTrades.Sum(t => (t.Comission + t.Swap));
            ViewBag.totalSwapComissions = totalSwapComissions.ToString("C0");

            // total acc balance no swap no comiussions 
            float totalIncome = selectedTrades.Sum(t => t.TradeOutcome);
            ViewBag.totalIncome = totalIncome.ToString("C0");

            // roznica 
            float totalISC = totalIncome - totalSwapComissions;
            ViewBag.totalISC = totalISC.ToString("C0");

            // Wykres okragly
            ViewBag.DoughnutChartData = selectedTrades
                .GroupBy(t => t.PositionType)
                .Select(g => new
                {
                    type = g.First().PositionType.ToString(), 
                    amount = g.Sum(t => t.TradeOutcome),
                    formattedAmount = g.Sum(t => t.TradeOutcome).ToString("C0")
                })
                .ToList();

            //wykres liniowy
            string[] Last7Days = Enumerable.Range(0, 7)
                .Select(i => DateTime.Now.AddDays(i).ToString("dd-MMM"))
                .ToArray();


            // profit
            List<SplineChartData> profitSummary = selectedTrades
                .Where(i => i.TradeOutcome > 0)
                .Select(j => new SplineChartData()
                {
                    date = j.TradeAdded.ToString("dd-MMM"), // Example balance based on date
                    profit = j.TradeOutcome,
                }).ToList();

            // loss
            List<SplineChartData> lossSummary = selectedTrades
                .Where(i => i.TradeOutcome < 0)
                .Select(j => new SplineChartData()
                {
                    date = j.TradeAdded.ToString("dd-MMM"), // Example balance based on date
                    loss = j.TradeOutcome
                }).ToList();

            ViewBag.SplineChartData = from day in Last7Days
                                      join profit in profitSummary on day equals profit.date into dayProfitJoined
                                      from profit in dayProfitJoined.DefaultIfEmpty()
                                      join loss in lossSummary on day equals loss.date into dayLossJoined
                                      from loss in dayLossJoined.DefaultIfEmpty()
                                      select new
                                      {
                                          date = day,
                                          profit = profit == null ? 0 : profit.profit,
                                          loss = loss == null ? 0 : loss.loss,
                                      };

            return View();
        }
    }
    public class SplineChartData
    {
        public string date;
        public float profit;
        public float loss;
    }
}

