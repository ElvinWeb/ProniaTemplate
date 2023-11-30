using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using ProniaProject.DataAccessLayer;
using ProniaProject.Helpers;
using ProniaProject.Models;

namespace ProniaProject.Areas.Manage.Controllers
{
    [Area("Manage")]
    public class ProductController : Controller
    {

        private readonly AppDbContext _DbContext;
        private readonly IWebHostEnvironment _env;
        public ProductController(AppDbContext _context, IWebHostEnvironment env)
        {
            _DbContext = _context;
            _env = env;
        }

        public IActionResult Index()
        {
            List<Product> products = _DbContext.Products.ToList();
            return View(products);

        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Colors = _DbContext.Colors.ToList();
            ViewBag.Categories = _DbContext.Categories.ToList();


            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            ViewBag.Colors = _DbContext.Colors.ToList();
            ViewBag.Categories = _DbContext.Categories.ToList();

            if (!ModelState.IsValid) return NotFound();

            if (!_DbContext.Categories.Any(a => a.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not found!");
                return View();
            }

            bool check = true;
            if (product.ColorIds != null)
            {
                foreach (var colorId in product.ColorIds)
                {
                    if (!_DbContext.Colors.Any(x => x.Id == colorId))
                    {
                        check = false;
                        break;
                    }
                }

            }
            if (check)
            {
                foreach (var colorId in product.ColorIds)
                {
                    ProductColor color = new ProductColor()
                    {
                        Product = product,
                        ColorId = colorId,
                    };
                    _DbContext.ProductColors.Add(color);
                }
            }
            else
            {
                ModelState.AddModelError("ColorIds", "id is not found!");
                return View();
            }

            if (product.ProductMainImage != null)
            {
                if (product.ProductMainImage.ContentType != "image/png" && product.ProductMainImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ProductMainImage", "error!");
                    return View();
                }
                if (product.ProductMainImage.Length > 1048576)
                {
                    ModelState.AddModelError("ProductMainImage", "error!");
                    return View();
                }

                string folder = "uploads/product-images";
                string newFileName = Helper.GetFileName(_env.WebRootPath, folder, product.ProductMainImage);
                ProductImage productImage = new ProductImage
                {
                    Product = product,
                    ImgUrl = newFileName,
                    isPoster = true,
                };
                _DbContext.ProductImages.Add(productImage);
            }
            if (product.ProductHoverImage != null)
            {
                if (product.ProductHoverImage.ContentType != "image/png" && product.ProductHoverImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ProductHoverImage", "error!");
                    return View();
                }
                if (product.ProductHoverImage.Length > 1048576)
                {
                    ModelState.AddModelError("ProductHoverImage", "error!");
                    return View();
                }

                string folder = "uploads/product-images";
                string newFileName = Helper.GetFileName(_env.WebRootPath, folder, product.ProductHoverImage);
                ProductImage productImage = new ProductImage
                {
                    Product = product,
                    ImgUrl = newFileName,
                    isPoster = false,
                };
                _DbContext.ProductImages.Add(productImage);
            }
            if (product.ImagesFiles != null)
            {
                foreach (var img in product.ImagesFiles)
                {

                    if (img.ContentType != "image/png" && img.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ProductHoverImage", "error!");
                        return View();
                    }
                    if (img.Length > 1048576)
                    {
                        ModelState.AddModelError("ProductHoverImage", "error!");
                        return View();
                    }

                    string folder = "uploads/product-images";
                    string newFileName = Helper.GetFileName(_env.WebRootPath, folder, img);

                    ProductImage productImage = new ProductImage
                    {
                        Product = product,
                        ImgUrl = newFileName,
                        isPoster = null,
                    };
                    _DbContext.ProductImages.Add(productImage);
                }
            }


            _DbContext.Products.Add(product);
            _DbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Update(int id)
        {
            ViewBag.Colors = _DbContext.Colors.ToList();
            ViewBag.Categories = _DbContext.Categories.ToList();

            if (id == null) return NotFound();

            Product product = _DbContext.Products.Include(pt => pt.ProductColors).Include(x => x.ProductImages).FirstOrDefault(p => p.Id == id);

            if (product == null) return NotFound();

            product.ColorIds = product.ProductColors.Select(t => t.ColorId).ToList();

            return View(product);
        }

        [HttpPost]
        public IActionResult Update(Product product)
        {
            ViewBag.Colors = _DbContext.Colors.ToList();
            ViewBag.Categories = _DbContext.Categories.ToList();

            //if (!ModelState.IsValid) return View();

            Product exitProduct = _DbContext.Products.Include(pt => pt.ProductColors).Include(x => x.ProductImages).FirstOrDefault(p => p.Id == product.Id);

            if (!_DbContext.Categories.Any(a => a.Id == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Category is not found!");
                return View();
            }

            exitProduct.ProductColors.RemoveAll(pc => !product.ColorIds.Any(id => id == pc.ColorId));

            foreach (var id in product.ColorIds.Where(pt => !exitProduct.ProductColors.Any(pId => pt == pId.ColorId)))
            {
                ProductColor productColor = new ProductColor()
                {
                    ProductId = id,
                };

                exitProduct.ProductColors.Add(productColor);
            }

            if (product.ProductMainImage != null)
            {
                string folderPath = "uploads/product-images";
                string path = Path.Combine(_env.WebRootPath, folderPath, exitProduct.ProductImages.FirstOrDefault(x => x.isPoster == true).ImgUrl);

                exitProduct.ProductImages.RemoveAll(bi => !product.ProductImageIds.Contains(bi.Id) && bi.isPoster == true);


                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                if (product.ProductMainImage.ContentType != "image/png" && product.ProductMainImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ProductMainImage", "please select correct file type");
                    return View();
                }

                if (product.ProductMainImage.Length > 1048576)
                {
                    ModelState.AddModelError("ProductMainImage", "file size should be more lower than 1mb ");
                    return View();
                }

                string newFileName = Helper.GetFileName(_env.WebRootPath, "uploads/product-images", product.ProductMainImage);

                ProductImage productImage = new ProductImage
                {
                    Product = product,
                    ImgUrl = newFileName,
                    isPoster = true,
                };
                exitProduct.ProductImages.Add(productImage);
            };
            if (product.ProductHoverImage != null)
            {
                string folderPath = "uploads/product-images";
                string path = Path.Combine(_env.WebRootPath, folderPath, exitProduct.ProductImages.FirstOrDefault(x => x.isPoster == false).ImgUrl);

                exitProduct.ProductImages.RemoveAll(bi => !product.ProductImageIds.Contains(bi.Id) && bi.isPoster == false);


                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                if (product.ProductHoverImage.ContentType != "image/png" && product.ProductHoverImage.ContentType != "image/jpeg")
                {
                    ModelState.AddModelError("ProductHoverImage", "please select correct file type");
                    return View();
                }

                if (product.ProductHoverImage.Length > 1048576)
                {
                    ModelState.AddModelError("ProductHoverImage", "file size should be more lower than 1mb ");
                    return View();
                }

                string newFileName = Helper.GetFileName(_env.WebRootPath, "uploads/product-images", product.ProductHoverImage);

                ProductImage productImage = new ProductImage
                {
                    Product = product,
                    ImgUrl = newFileName,
                    isPoster = true,
                };
                exitProduct.ProductImages.Add(productImage);
            };

            foreach (var item in exitProduct.ProductImages.Where(x => !product.ProductImageIds.Contains(x.Id) && x.isPoster == null))
            {
                string fullPath = Path.Combine(_env.WebRootPath, "uploads/product-images", item.ImgUrl);

                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }

            exitProduct.ProductImages.RemoveAll(pi => !product.ProductImageIds.Contains(pi.Id) && pi.isPoster == null);

            if (product.ImagesFiles != null)
            {
                foreach (var img in product.ImagesFiles)
                {

                    if (img.ContentType != "image/png" && img.ContentType != "image/jpeg")
                    {
                        ModelState.AddModelError("ProductHoverImage", "error!");
                        return View();
                    }
                    if (img.Length > 1048576)
                    {
                        ModelState.AddModelError("ProductHoverImage", "error!");
                        return View();
                    }

                    string folder = "uploads/product-images";
                    string newFileName = Helper.GetFileName(_env.WebRootPath, folder, img);

                    ProductImage productImage = new ProductImage
                    {
                        Product = product,
                        ImgUrl = newFileName,
                        isPoster = null,
                    };
                    exitProduct.ProductImages.Add(productImage);
                }
            }

            exitProduct.Name = product.Name;
            exitProduct.Description = product.Description;
            exitProduct.SalePrice = product.SalePrice;
            exitProduct.CategoryId = product.CategoryId;

            _DbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            ViewBag.Colors = _DbContext.Colors.ToList();
            ViewBag.Categories = _DbContext.Categories.ToList();

            if (id == null) NotFound();

            Product product = _DbContext.Products.Include(p => p.ProductImages).FirstOrDefault(p => p.Id == id);


            if (product == null) NotFound();

            if (product.ProductImages != null)
            {

                foreach (var item in product.ProductImages)
                {
                    if (item.isPoster == true)
                    {
                        string fullPath = Path.Combine(_env.WebRootPath, "uploads/product-images", item.ImgUrl);

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                    }
                    if (item.isPoster == false)
                    {
                        string fullPath = Path.Combine(_env.WebRootPath, "uploads/product-images", item.ImgUrl);

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                    }
                    if (item.isPoster == null)
                    {
                        string fullPath = Path.Combine(_env.WebRootPath, "uploads/product-images", item.ImgUrl);

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }

                    }
                }
            }

            _DbContext.Products.Remove(product);
            _DbContext.SaveChanges();

            return Ok();
        }
    }
}
