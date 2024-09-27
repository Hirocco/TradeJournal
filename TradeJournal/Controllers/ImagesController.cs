using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;
using TradeJournal.ViewModels;

namespace TradeJournal.Controllers
{
    public class ImagesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostingEnv;

        public ImagesController(AppDbContext context, IWebHostEnvironment hostingEnv)
        {
            _context = context;
            _hostingEnv = hostingEnv;
        }

        public async Task<IActionResult> Details(int? id)
        {
            var image = await _context.Image.FirstOrDefaultAsync(i => i.Id == id);
            if (image == null) return NotFound();

            // Zakodowany obraz Base64
            string base64Image = image.FileContent;

            // Przekazanie zawartości obrazu i tytułu do ViewBag
            ViewBag.ImageContent = "data:image/PNG;base64," + base64Image;
            ViewBag.ImageTitle = image.Title;

            return View(image);
        }


        // GET: Images/Create
        public IActionResult AddImage()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddImage([Bind("Id,Title,ImageFile,TradeId")] Image image)
        {
            // Sprawdź, czy plik został przesłany
            if (image.ImageFile != null && image.ImageFile.Length > 0)
            {
                using var ms = new MemoryStream();
                await image.ImageFile.CopyToAsync(ms);
                byte[] fileBytes = ms.ToArray();
                image.FileContent = Convert.ToBase64String(fileBytes);
            }
            else
            {
                ModelState.AddModelError("ImageFile", "Image file is required.");
                return RedirectToAction("Details", "Trades", new { id = image.TradeId });
            }

            Console.WriteLine($"FILE CONTENT: {image.FileContent}");

            ModelState.ClearValidationState(nameof(Image));
            if (ModelState.IsValid)
            {
                // insert record
                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Trades", new {id = image.TradeId});
            }
            return RedirectToAction("Details", "Trades",new { image.TradeId });
        }

        // POST: Images/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var image = await _context.Image.FindAsync(id);
            if (image != null)
            {
                _context.Image.Remove(image);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImageExists(int id)
        {
            return _context.Image.Any(e => e.Id == id);
        }
    }
}
