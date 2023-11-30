using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProniaProject.DataAccessLayer;
using ProniaProject.ViewModels;

namespace ProniaProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _DbContext;
        public HomeController(AppDbContext _context)
        {
            _DbContext = _context;
        }
        public IActionResult Index()
        {
            HomeViewModel model = new HomeViewModel()
            {
                FeaturedProducts = _DbContext.Products.Include(p => p.ProductImages).Where(x => x.isFeatured).ToList(),
                BestsellerProducts = _DbContext.Products.Include(p => p.ProductImages).Where(x => x.isBestseller).ToList(),
                LatestProducts = _DbContext.Products.Include(p => p.ProductImages).Where(x => x.isLatest).ToList(),
            };
            
            return View(model);
        }
    }
}
