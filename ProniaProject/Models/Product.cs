using System.ComponentModel.DataAnnotations.Schema;

namespace ProniaProject.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int SalePrice { get; set; }
        public bool isFeatured { get; set; }
        public bool isBestseller { get; set; }
        public bool isLatest { get; set; }
        public Category? Category { get; set; }
        public int CategoryId { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public List<ProductColor>? ProductColors { get; set; }
        [NotMapped]
        public IFormFile? ProductMainImage { get; set; }
        [NotMapped]
        public IFormFile? ProductHoverImage { get; set; }
        [NotMapped]
        public List<IFormFile>? ImagesFiles { get; set; }
        [NotMapped]
        public List<int>? ProductImageIds { get; set; }
        [NotMapped]
        public List<int>? ColorIds { get; set; }
    }
}
