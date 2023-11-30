using Microsoft.AspNetCore.Mvc;
using ProniaProject.DataAccessLayer;
using ProniaProject.Models;

namespace ProniaProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ColorController : Controller
    {
        private readonly AppDbContext _DbContext;
        public ColorController(AppDbContext _context)
        {
            _DbContext = _context;
        }

        public IActionResult Index()
        {
            List<Color> colors = _DbContext.Colors.ToList();
            return View(colors);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Color color)
        {
            if (!ModelState.IsValid) return View();

            if (_DbContext.Categories.Any(k => k.Name.ToLower() == color.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "color has already created!");
                return View();
            }

            _DbContext.Colors.Add(color);
            _DbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            if (id == null) return NotFound();

            Color color = _DbContext.Colors.FirstOrDefault(k => k.Id == id);

            if (color == null) return NotFound();

            return View(color);
        }

        [HttpPost]
        public IActionResult Update(Color color)
        {
            if (!ModelState.IsValid) return View();

            Color existColor = _DbContext.Colors.FirstOrDefault(k => k.Id == color.Id);

            if (existColor == null) return NotFound();

            if (_DbContext.Colors.Any(k => k.Id != color.Id && k.Name.ToLower() == color.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "color has already created!");
                return View();
            }

            existColor.Name = color.Name;

            _DbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null) return NotFound();

            Color color = _DbContext.Colors.FirstOrDefault(k => k.Id == id);

            if (color == null) return NotFound();

            _DbContext.Colors.Remove(color);
            _DbContext.SaveChanges();

            return Ok();
        }
    }
}
