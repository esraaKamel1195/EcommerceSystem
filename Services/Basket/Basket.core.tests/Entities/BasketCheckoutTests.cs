using Basket.Core.Entities;

namespace Basket.core.tests.Entities
{
    public class BasketCheckoutTests
    {
        [Fact]
        public void BasketCheckout_CanBeCreated()
        {
            var checkout = new BasketCheckout();

            Assert.NotNull(checkout);
        }

        [Fact]
        public void BasketCheckout_TotalPrice_NotBeZeroOrNegative()
        {
            Assert.Throws<ArgumentException>(() => 
            {
                var checkout = new BasketCheckout() { TotalPrice = 0 };
            });

            Assert.Throws<ArgumentException>(() =>
            {
                var checkout2 = new BasketCheckout() { TotalPrice = -10 };
            });
        }
    }
}
