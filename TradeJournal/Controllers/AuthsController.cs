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
                try
                {
                    var tokenDto = await _userService.LoginAsync(new AuthDTO
                    {
                        Email = auth.Email,
                        Password = auth.Password
                    });
                    //HttpContext.Session.SetString("key", "abcdefg");

                    return View("~/Views/Dashboard/Index.cshtml");
                }
                catch (UnauthorizedAccessException e)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    throw new Exception(e.Message);
                }
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
                try
                {
                    await _userService.RegisterUserAsync(new UserRegisterDTO
                    {
                        Login = userRegister.Login,
                        Email = userRegister.Email,
                        Password = userRegister.Password
                    });
                }
                catch(Exception e)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    throw new Exception(e.Message);
                }

            }
            return View(nameof(Register));

        }

    }
}
