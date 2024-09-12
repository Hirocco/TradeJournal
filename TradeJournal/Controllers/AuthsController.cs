using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Data.DTOs;
using TradeJournal.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using TradeJournal.Services.user;
using Azure;

namespace TradeJournal.Controllers
{
    public class AuthsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IUserService _userService;

        public AuthsController(AppDbContext context, HttpClient httpClient, IUserService userService)
        {
            _context = context;
            _httpClient = httpClient;   
            _userService = userService;
        }

        public IActionResult Login() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email, Password")] AuthDTO auth)
        {
            Console.WriteLine($"HttpPost Login: {auth.Email} , {auth.Password}");
            if (ModelState.IsValid)
            {
                var apiCallUrl = "https://localhost:7098/api/login";

                // Serializuj dane z formularza do formatu JSON
                var jsonContent = JsonSerializer.Serialize(new
                {
                    Email = auth.Email,
                    Password = auth.Password
                });
                
               // Tworzymy zawartość żądania HTTP
               var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                // Wykonujemy żądanie POST do API
                //var response = await _httpClient.PostAsync(apiCallUrl, content);
                var temp = new AuthDTO
                {
                    Email = auth.Email,
                    Password = auth.Password
                };

                try
                {
                    var tokenDto = await _userService.LoginAsync(temp);
                    HttpContext.Session.SetString("key", "abcdefg");

                    return View("~/Views/Dashboard/Index.cshtml");
                }
                catch (UnauthorizedAccessException e) {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    //Unauthorized(e.Message); 
                }

                /*
                //Console.WriteLine($"Dane: {jsonContent} - {content} - {response}");
                // Sprawdzamy, czy żądanie zakończyło się sukcesem
                if (log.)
                {
                    var tokenJson = await response.Content.ReadAsStringAsync();
                    var token = JsonSerializer.Deserialize<TokenDTO>(tokenJson);

                    HttpContext.Session.SetString("key", "abcdefg");

                    return View("~/Views/Dashboard/Index.cshtml");
                }
                else
                {
                    // Obsługa błędu
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }*/
            }

            // Jeśli niepowodzenie, zwróć widok login z błędami
            return View(nameof(Login));
        }

        public IActionResult Register() { return View(); }

        [HttpPost]
        public async Task<IActionResult> Register([Bind("Login, Email, Password")] UserRegisterDTO userRegister)
        {
            Console.WriteLine($"{userRegister.Login} - {userRegister.Email} - {userRegister.Password}");

            if(ModelState.IsValid)
            {
                var apiCallUrl = "https://localhost:7098/api/register";
                var jsonContent = JsonSerializer.Serialize(new
                {
                    Login = userRegister.Login,
                    Email = userRegister.Email,
                    Password = userRegister.Password,
                });
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(apiCallUrl, content);

                Console.WriteLine($"Dane register: {jsonContent} - {content} - {response}");

                if (response.IsSuccessStatusCode)   return View(nameof(Login));
                else ModelState.AddModelError(string.Empty, "Invalid login attempt.");

            }
            return View(nameof(Register));

        }

    }
}
