

using Basket.Application.Handlers.Commands;
using Basket.Core.Repositories;
using Moq;

namespace Basket.application.tests.Commands
{
    public class DeleteBasketByUserNameCommandTests
    {
        [Fact]
        public async Task Handler_DeleteBasketByUsername_Valid()
        {
            //arrange
            var mockBasketRepo = new Mock<IBasketRepository>();

            var username = "esraa";

            mockBasketRepo.Setup(r => r.DeleteBasket(username)).Returns(Task.CompletedTask);

            var handler = new DeleteBasketByUserNameCommandHandler(mockBasketRepo.Object);

            //act
            await handler.Handle(
                new Application.Commands.DeleteBasketByUserNameCommand(username), new CancellationToken()
            );

            //assert
            mockBasketRepo.Verify(r => r.DeleteBasket(username), Times.Once);
        }
    }
}
