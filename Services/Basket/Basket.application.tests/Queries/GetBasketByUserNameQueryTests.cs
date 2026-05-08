using AutoMapper;
using Basket.Application.Handlers.Queries;
using Basket.Application.Mappers;
using Basket.Application.Queries;
using Basket.Core.Entities;
using Basket.Core.Repositories;
using Moq;

namespace Basket.application.tests.Queries
{
    public class GetBasketByUserNameQueryTests
    {
        [Fact]
        public async Task Handle_WhenBasketExist_ReturnBasket()
        {
            //Arrange
            var mockRepo = new Mock<IBasketRepository>();

            var username = "Esraa";

            var basket = new ShoppingCart(username);

            mockRepo.Setup(r => r.GetBasket(username)).ReturnsAsync(basket);

            var mapperConfig = new MapperConfiguration(cfg => 
            {
                cfg.AddProfile<BasketMappingProfile>();
            });

            IMapper mapper = mapperConfig.CreateMapper();

            var handler = new GetBasketByUserNameQueryHandler(mockRepo.Object, mapper);

            //act
            var result = await handler.Handle(new GetBasketByUserNameQuery(username), new CancellationToken());

            //assert
            Assert.NotNull(result);
            Assert.Equal(username, result.UserName);
        }

        [Fact]
        public async Task Handle_WhenBasketDoesNotExist_ReturnNull()
        {
            var mockRepository = new Mock<IBasketRepository>();

            mockRepository.Setup(r => r.GetBasket(It.IsAny<string>())).ReturnsAsync((ShoppingCart)null);

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<BasketMappingProfile>();
            });

            IMapper mapper = mapperConfig.CreateMapper();

            var handler = new GetBasketByUserNameQueryHandler(mockRepository.Object, mapper);

            var result = await handler.Handle(new GetBasketByUserNameQuery(It.IsAny<string>()), new CancellationToken());

            Assert.Null(result);
        }
    }
}
