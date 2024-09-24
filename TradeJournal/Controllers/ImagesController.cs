using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TradeJournal.Data;
using TradeJournal.Models;

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
            var image = _context.Image.FirstOrDefault(i => i.Id == id);
            if (image == null) return NotFound();

            ViewBag.ImagePath = Url.Content("~/Image/" + image.ImageName);
            ViewBag.ImageTitle = image.Title;

            return View(image);
        }

        // GET: Images/Create
        public IActionResult AddImage()
        {
            return View();
        }



        [HttpPost]
        public async Task<IActionResult> AddImage([Bind("Id,Title,ImageFile")] Image image)
        {
            if (ModelState.IsValid)
            {
                // save image to wwwroot/image
                string wwwRootPath = _hostingEnv.WebRootPath;
                string fileName = Path.GetFileNameWithoutExtension(image.ImageFile.FileName);
                string extension = Path.GetExtension(image.ImageFile.FileName);

                fileName = fileName + DateTime.Now.ToString("yymmsfff") + extension;
                string path = Path.Combine(wwwRootPath + "/Image", fileName);

                image.ImageName = fileName;

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await image.ImageFile.CopyToAsync(fileStream);
                }

                // insert record
                _context.Add(image);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
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
