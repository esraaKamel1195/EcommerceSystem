

namespace Basket.Core.Entities
{
    public class ShoppingCartItem
    {
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string productId { get; set; }
        public string productName { get; set; }
        public string ImageFile { get; set; }
    }
}
