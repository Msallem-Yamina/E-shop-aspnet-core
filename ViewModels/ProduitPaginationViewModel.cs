using TP_Product.Models;

namespace TP_Product.ViewModels
{
    public class ProduitPaginationViewModel
    {
        public List<Product> Products { get; set; }
        public int PageActuelle { get; set; }
        public int TotalPages { get; set; }
    }
}