namespace Ecommerce_Webapp.Models
{
    public class AddProductViewModel
    {
        public string Pname { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public List<IFormFile> Images { get; set; }
    }
}
