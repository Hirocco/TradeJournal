using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.Patterns;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TradeJournal.Data;
using TradeJournal.Services.session;
using TradeJournal.Services.token;
using TradeJournal.Services.user;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// DB
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DevConnection")));

// SESJA
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Session.Works";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Czas wygaœniêcia sesji
    options.Cookie.IsEssential = true; // Sesje bêd¹ dzia³aæ nawet bez zgody na cookies
    options.Cookie.SameSite = SameSiteMode.Lax;     // Pozwala na prawid³owe przekazywanie ciasteczka
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

// LICENCJA SYNCFUSION
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Mgo+DSMBaFt5QHFqVkNrXVNbdV5dVGpAd0N3RGlcdlR1fUUmHVdTRHRbQlVjQX5Uc0JiUHpaeHc=;Mgo+DSMBPh8sVXJyS0d+X1RPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9nSXZTfkVkXHhfeXFTQmA=;Mgo+DSMBMAY9C3t2U1hhQlJBfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hTX5bdk1jXnpZcX1QQGRc;MzQyMzA3NUAzMjM2MmUzMDJlMzBPb3F5bU10SHhja2R4L0pqOGtkOFVWdnhBbitTQzd0bTBnaTBEUTRmUmRFPQ==");

// IDENTITY
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// SERVICE
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();

// JWT
builder.Services.AddScoped<IJwtTokenService>(provider =>
    new JwtService(
        builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"],
        builder.Configuration["Jwt:Audience"],
        int.Parse(builder.Configuration["Jwt:ExpiryMinutes"])));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Kolejnoœæ middleware
app.UseRouting();
app.UseSession(); // <-- Sesja musi byæ przed autoryzacj¹ i autentykacj¹
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseCookiePolicy();


app.UseAuthentication(); // <-- Musi byæ przed autoryzacj¹
app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auths}/{action=Login}/{id?}");

app.Run();
