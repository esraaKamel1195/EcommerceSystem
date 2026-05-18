using Catalog.Core.Entities;

namespace Catalog.Core.Tests
{
    public class ProductTests
    {
        [Fact]
        public void CreateProduct_ShouldReturnValidProduct()
        {
            // Arrange
            var product = new Product
            {
                Id = "1",
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 9.99m,
                ImageFile = "test.svg",
                Brands = new ProductBrand
                {
                    Id = "brand1",
                    Name = "Test Brand"
                },
                Types = new ProductType
                {
                    Id = "type1",
                    Name = "Test Type"
                },
                HasDiscount = false,
                DiscountAmount = 0,
                PriceAfterDiscount = null,
            };

            // Assert
            Assert.Equal("1", product.Id);
            Assert.Equal("Test Product", product.Name);
            Assert.Equal("This is a test product.", product.Description);
            Assert.Equal(9.99m, product.Price);
            Assert.Equal("test.svg", product.ImageFile);
            Assert.Equal(new ProductBrand
            {
                Id = "brand1",
                Name = "Test Brand"
            }, product.Brands);
            Assert.Equal(new ProductType
            {
                Id = "type1",
                Name = "Test Type"
            }, product.Types);
        }

        [Fact]
        public void CheckProduct_IfHasDiscount_AmountBEGreaterThanZero() {
            // Arrange
            var product = new Product
            {
                Id = "1",
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 99.99m,
                ImageFile = "test.svg",
                Brands = new ProductBrand
                {
                    Id = "brand1",
                    Name = "Test Brand"
                },
                Types = new ProductType
                {
                    Id = "type1",
                    Name = "Test Type"
                },
                HasDiscount = true,
                DiscountAmount = 10,
                PriceAfterDiscount = 88.88m,
            };

            //assert
            Assert.True(product.HasDiscount);
            Assert.Equal(10, product.DiscountAmount);
        }

        [Fact]
        public void CheckProduct_IfDoesNotHasDiscount_AmountBEZero()
        {
            // Arrange
            var product = new Product
            {
                Id = "1",
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 9.99m,
                ImageFile = "test.svg",
                Brands = new ProductBrand
                {
                    Id = "brand1",
                    Name = "Test Brand"
                },
                Types = new ProductType
                {
                    Id = "type1",
                    Name = "Test Type"
                },
                HasDiscount = false,
                DiscountAmount = 0,
                PriceAfterDiscount = null,
            };

            Assert.Equal(0, product.DiscountAmount);
        }

        [Fact]
        public void CheckProduct_IfNoDiscount_PriceAfterDiscountBeNull()
        {
            // Arrange
            var product = new Product
            {
                Id = "1",
                Name = "Test Product",
                Description = "This is a test product.",
                Price = 9.99m,
                ImageFile = "test.svg",
                Brands = new ProductBrand
                {
                    Id = "brand1",
                    Name = "Test Brand"
                },
                Types = new ProductType
                {
                    Id = "type1",
                    Name = "Test Type"
                },
                HasDiscount = false,
                DiscountAmount = 0,
                PriceAfterDiscount = null,
            };

            // Assert
            Assert.False(product.HasDiscount);
            Assert.Null(product.PriceAfterDiscount);
        }
    }
}