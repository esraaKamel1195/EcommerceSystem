
namespace Basket.Application.Responses
{
    public class ShoppingCartItemResponse
    {
        public int quantity { get; set; }
        public decimal price { get; set; }
        public string productId { get; set; }

        public string productName { get; set; }

        public string ImageFile { get; set; }
    }
}
