namespace Basket.Core.Entities
{
    public class BasketCheckout
    {
        private decimal _totalPrice;
        public string Username { get; set; }
        public decimal TotalPrice { 
            get => _totalPrice; 
            set 
            { if (value <= 0) 
                 throw new ArgumentException("Total Price cannot be negative or zero");
                 _totalPrice = value;
            }
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string? ZipCode { get; set; }
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public string PaymentMethod { get; set; }
    }
}