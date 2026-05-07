
using System.ComponentModel.DataAnnotations;

namespace Basket.Core.Entities
{
    public class ShoppingCartItem
    {
        public string productId { get; set; }
        public string productName { get; set; }
        public string ImageFile { get; set; }
        [Range(0, int.MaxValue, ErrorMessage = "Quantity shouldn't be negative")]
        public int quantity { get; set; }
        public decimal price { get; set; }
        public decimal? priceAfterDiscount { get; set; } = null;
    }
}
