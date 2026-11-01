using System.ComponentModel.DataAnnotations;

namespace TP_Product.ViewModels
{
    public class OrderViewModel
    {
        [Required(ErrorMessage = "L'adresse de livraison est obligatoire.")]
        public string Address { get; set; }

        public string PaymentMethod { get; set; }
        public float TotalAmount { get; set; }
        public List<CartItemViewModel> CartItems { get; set; }
    }
}