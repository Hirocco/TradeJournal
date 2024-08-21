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

            //wykres liniowy ostatniego miesiaca
            string[] LastMonth = Enumerable.Range(0, 31)
                .Select(i => DateTime.Today.AddDays(-30).AddDays(i).ToString("dd-MMM"))
                .ToArray();

            string[] LastQuarter = Enumerable.Range(0, 92)
                .Select(i => DateTime.Today.AddDays(-91).AddDays(i).ToString("dd-MMM"))
                .ToArray();

            string[] LastWeek = Enumerable.Range(0, 7)
                .Select(i => DateTime.Today.AddDays(-6).AddDays(i).ToString("dd-MMM"))
                .ToArray();


            // total profit
            List<SplineChartData> profitSummary = selectedTrades
                .Where(i => i.TradeOutcome > 0)
                .Select(j => new SplineChartData()
                {
                    date = j.TradeAdded.ToString("dd-MMM"), // Example balance based on date
                    profit = j.TradeOutcome,
                }).ToList();

            // total loss
            float absLoss = 0;
            List<SplineChartData> lossSummary = selectedTrades
                .Where(i => i.TradeOutcome < 0)
                .Select(j => 
                {
                    absLoss = Math.Abs(j.TradeOutcome);
                    return new SplineChartData()
                    {
                        date = j.TradeAdded.ToString("dd-MMM"), // Example balance based on date
                        loss = absLoss
                    };
                }).ToList();

            /*Wyswietlanie profitow i strat*/
            ViewBag.SplineChartData = from day in LastMonth
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


            
            // total 
            // Etap 1: Obliczanie kumulatywnego bilansu
            float cumulativeBalance = 0; // Zmienna przechowująca kumulatywny bilans

            List<SplineChartDataBalance> totalNetBalance = selectedTrades
               .Select(t =>
               {
                   cumulativeBalance += t.TradeOutcome; // Aktualizuj kumulatywny bilans

                   return new SplineChartDataBalance()
                   {
                       Date = t.TransactionCloseDate.ToString("dd-MMM"),
                       NetBalance = cumulativeBalance // Przypisz aktualny kumulatywny bilans
                   };
               }).ToList();

            // Etap 2: Przetwarzanie danych w celu zachowania ostatniego bilansu w dniach bez transakcji
            cumulativeBalance = totalNetBalance.Last().NetBalance; // Zainicjuj kumulatywny bilans ostatnią wartością z poprzedniego etapu

            ViewBag.SplineChartDataBalance = LastMonth
               .Select(day =>
               {
                   var balance = totalNetBalance.FirstOrDefault(b => b.Date == day);
                   if (balance == null)
                   {
                       return new SplineChartDataBalance()
                       {
                           Date = day,
                           NetBalance = cumulativeBalance // Utrzymanie ostatniego znanego bilansu
                       };
                   }
                   else
                   {
                       cumulativeBalance = balance.NetBalance; // Aktualizacja bilansu na ten dzień
                       return balance;
                   }
               }).ToList();



            //recent trades
            ViewBag.recentTrades = await _appDbContext.Trades
               .OrderByDescending(j => j.TradeAdded)
               .Take(7)
               .ToListAsync();

          


            return View();

        }
    }

    public class SplineChartDataBalance
    {
        public string Date { get; set; }
        public float NetBalance { get; set; } = 0;
    } 
    
    public class SplineChartData
    {
        public string date;
        public float profit;
        public float loss;
    }
}

