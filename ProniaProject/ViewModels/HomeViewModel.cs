using ProniaProject.Models;

namespace ProniaProject.ViewModels
{
    public class HomeViewModel
    {
        public List<Product> BestsellerProducts { get; set; }
        public List<Product> FeaturedProducts { get; set; }
        public List<Product> LatestProducts { get; set; }

    }
}
