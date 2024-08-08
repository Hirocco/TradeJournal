using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt5QHFqVkNrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRbQlVjQX5Uc0JiUHpaeHc=;Mgo+DSMBPh8sVXJyS0d+X1RPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9nSXZTfkVkXHhfeXFTQmA=;Mgo+DSMBMAY9C3t2U1hhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTX5bdk1jXnpZcX1QQGRc;MzQyMzA3NUAzMjM2MmUzMDJlMzBPb3F5bU10SHhja2R4L0pqOGtkOFVWdnhBbitTQzd0bTBnaTBEUTRmUmRFPQ==");
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
