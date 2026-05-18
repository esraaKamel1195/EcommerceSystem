using Basket.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Basket.core.tests.Entities
{
    public class ShoppingCartItemTests
    {
        [Fact]
        public void ShoppingCartItem_testProperty_CorrectValue()
        {
            //arrange
            var cartItem = new ShoppingCartItem()
            {
                productId = "1",
                productName = "Test Product",
                ImageFile = "test.svg",
                quantity = 1,
                price = 100,
            };

            //assert
            Assert.Equal("1", cartItem.productId);
            Assert.Equal("Test Product", cartItem.productName);
            Assert.Equal(1, cartItem.quantity);
            Assert.Equal(100, cartItem.price);
            Assert.Equal("test.svg", cartItem.ImageFile);
        }

        [Fact]
        public void ShoppingCartItem_TestQuantity_NotToBeNegatives() 
        {
            var shoppingCartItem = new ShoppingCartItem() { quantity = -1 };

            var context = new ValidationContext(shoppingCartItem);
            var result = new List<ValidationResult>();

            var isInvalid = Validator.TryValidateObject(shoppingCartItem, context, result, true);

            Assert.False(isInvalid);
        }

        [Fact]
        public void shoppingCartItem_TestPrice_PreservedDecimalPrecision()
        {
            var shoppingCartItem = new ShoppingCartItem() { price = 9.99m };

            Assert.Equal(9.99m, shoppingCartItem.price);
        }
    }
}
