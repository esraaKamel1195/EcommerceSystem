using Basket.Core.Entities;


namespace Basket.core.tests.Entities
{
    public class ShoppingCartTests
    {
        [Fact]
        public void ShoppingCart_DefaultConstructor_InitializesItems()
        {
            //AAA

            //Arrange
            var cart = new ShoppingCart();

            //Assert
            Assert.NotNull(cart.Items);
        }

        [Fact]
        public void ShoppingCart_WithUsername_SetsUsername()
        {
            //AAA

            //Arrange
            var username = "Esraa";

            var cart = new ShoppingCart(username);

            //Assert
            Assert.Equal(username, cart.UserName);
        }

        [Fact]
        public void ShoppingCart_Items_ShouldNeverBeNull()
        {
            // Arrange
            var cart = new ShoppingCart("Esraa");

            // assert
            Assert.NotNull(cart.Items);
        }

        [Fact]
        public void ShoppingCart_Items_InitializeItemsWithLengthZero()
        {
            //AAA 

            //Arrange
            var cart = new ShoppingCart();

            //assert
            Assert.Empty(cart.Items);
        }

        [Fact]
        public void shoppingCart_Items_CanAddItems()
        {
            //AAA pattern
            //Arrange
            var cart = new ShoppingCart("Esraa");

            var item = new ShoppingCartItem()
            {
                productId = "1",
                productName = "Test Product",
                ImageFile = "",
                quantity = 1,
                price = 100,
            };

            //Act
            cart.Items.Add(item);

            //Assert - verify
            Assert.Single(cart.Items);
            Assert.Equal(cart.Items[0], item);
        }

        [Fact]
        public void ShoppingCart_Instances_ShouldNotShareItem()
        {
            var cart1 = new ShoppingCart("Esraa");
            var cart2 = new ShoppingCart("Esraa2");

            cart1.Items.Add(new ShoppingCartItem());

            Assert.Single(cart1.Items);
            Assert.Empty(cart2.Items);
        }

        [Fact]
        public void ShoppingCart_CanBeSerialized()
        {
            var cart = new ShoppingCart("Esraa");

            var json = System.Text.Json.JsonSerializer.Serialize(cart);

            Assert.False(string.IsNullOrWhiteSpace(json));
        }

        [Fact]
        public void ShoppingCart_ShouldNotThrowException()
        {
            var cart = new ShoppingCart("Esraa");

            var exception = Record.Exception(() =>
            {
                var count = cart.Items.Count;
            });

            Assert.Null(exception);
        }
    }
}
