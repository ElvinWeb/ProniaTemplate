using Humanizer.Localisation;
using Microsoft.AspNetCore.Mvc;
using ProniaProject.DataAccessLayer;
using ProniaProject.Models;

namespace ProniaProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class CategoryController : Controller
    {
        private readonly AppDbContext _DbContext;
        public CategoryController(AppDbContext _context)
        {
            _DbContext = _context;
        }

        public IActionResult Index()
        {
            List<Category> categories = _DbContext.Categories.ToList();
            return View(categories);   
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (!ModelState.IsValid) return View();

            if (_DbContext.Categories.Any(k => k.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "category has already created!");
                return View();
            }

            _DbContext.Categories.Add(category);
            _DbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            if (id == null) return NotFound();

            Category category = _DbContext.Categories.FirstOrDefault(k => k.Id == id);

            if (category == null) return NotFound();

            return View(category);
        }

        [HttpPost]
        public IActionResult Update(Category category)
        {
            if (!ModelState.IsValid) return View();

            Category existCategory = _DbContext.Categories.FirstOrDefault(k => k.Id == category.Id);

            if (existCategory == null) return NotFound();

            if (_DbContext.Categories.Any(k => k.Id != category.Id && k.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "category has already created!");
                return View();
            }

            existCategory.Name = category.Name;

            _DbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (id == null) return NotFound();

            Category category = _DbContext.Categories.FirstOrDefault(k => k.Id == id);

            if (category == null) return NotFound();    

            _DbContext.Categories.Remove(category);
            _DbContext.SaveChanges();

            return Ok();
        }
    }
}
