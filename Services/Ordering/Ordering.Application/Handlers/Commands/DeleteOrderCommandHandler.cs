using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Extensions;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers.Commands
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;
        public DeleteOrderCommandHandler(IOrderRepository orderRepository, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var orderToDelete = await _orderRepository.GetByIdAsync(request.Id);
            if(orderToDelete == null)
            {
                throw new OrderNotFoundException(nameof(Order), request.Id);
            }
            await _orderRepository.DeleteAsync(orderToDelete);
            _logger.LogInformation("Order {Id} is successfully deleted.", request.Id);
            return Unit.Value;
        }
    }
}
