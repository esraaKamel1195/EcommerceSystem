using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.GRPCServices;
using Basket.Application.Handlers.Commands;
using Basket.Application.Responses;
using Basket.Core.Entities;
using Basket.Core.Repositories;
using Discount.Grpc.Protos;
using Moq;

namespace Basket.application.tests.Commands
{
    public class FakeDiscountGrpcService : DiscountGrpcService
    {
        private readonly CouponModel _coupon;
        public FakeDiscountGrpcService(CouponModel coupon): base(null!)
        {
            _coupon = coupon;
        }

        public override Task<CouponModel?> GetDiscount(string productName) 
        {
            return Task.FromResult<CouponModel>(_coupon);
        }
    }

    public class CreateShoppingCartCommandTests
    {
        [Fact]
        public async Task Handle_ValidCommand_CreateBasketAndReturnResponse() 
        {
            // arrange
            var mockBasketRepo = new Mock<IBasketRepository>();

            var mockMapper = new Mock<IMapper>();

            //var mockDiscountService = new Mock<DiscountGrpcService>();

            var items = new List<ShoppingCartItem>() 
            {
               new ShoppingCartItem { 
                  productName = "Iphone",
                  price= 900
               }
            };

            var command = new CreateShoppingCartCommand("esraa", items);

            var coupon = new CouponModel()
            {
                ProductName = "Iphone",
                Amount = 100
            };

            var discountService = new FakeDiscountGrpcService(coupon);

            //mockDiscountService.Setup(s => s.GetDiscount("Iphone")).ReturnsAsync(coupon);

            mockBasketRepo.Setup(r => r.UpdateBasket(It.IsAny<ShoppingCart>()))
                .ReturnsAsync((ShoppingCart cart) => cart);

            var expectedResponse = new ShoppingCartResponse { UserName = "esraa" };

            mockMapper.Setup(m => m.Map<ShoppingCartResponse>(It.IsAny<ShoppingCart>()))
                .Returns(expectedResponse);

            var handler = new CreateShoppingCartCommandHandler(
                mockBasketRepo.Object,
                mockMapper.Object,
                discountService
                );

            // act
            var result = await handler.Handle(command, CancellationToken.None);

            // assert
            Assert.NotNull(result);

            Assert.Equal("esraa", result.UserName);

            Assert.Empty(result.Items);
            //Assert.Equal(900, result.Items.First().price);

            //price after discount
            //mockDiscountService.Verify(s => s.GetDiscount("Iphone"), Times.Once());

            mockBasketRepo.Verify(
                r => r.UpdateBasket(It.Is<ShoppingCart>(c => 
                    c.UserName == "esraa" && c.Items.First().price == 900
                    )), Times.Once
            );

            mockMapper.Verify(
                m => m.Map<ShoppingCartResponse>(It.IsAny<ShoppingCart>()), Times.Once
            );

            mockBasketRepo.VerifyNoOtherCalls();
        }
    }
}
