using CsvHelper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TradeJournal.Data;
using TradeJournal.Models;

public class ImportTrade
{
    private readonly AppDbContext _context;

    public ImportTrade(AppDbContext context)
    {
        _context = context;
    }

    public async Task ImportCsvAsync(string filePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        var records = csv.GetRecords<Trade>().ToList();

        foreach (var record in records)
        {
            _context.Trades.Add(record);
        }

        await _context.SaveChangesAsync();
    }
}
