namespace ProniaProject.Models
{
    public class ProductImage
    {
        public int Id { get; set; }
        public string ImgUrl { get; set; }
        public int ProductId { get; set; }
        public bool? isPoster { get; set; }
        public Product? Product { get; set; }

    }
}
