using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Commands;
using Ordering.Application.Extensions;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;

namespace Ordering.Application.Handlers.Commands
{
    public class UpdateOrderCommandHandler : IRequestHandler<UpdateOrderCommand, Unit>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CheckoutOrderCommandHandler> _logger;

        public UpdateOrderCommandHandler(IOrderRepository orderRepository, IMapper mapper, ILogger<CheckoutOrderCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            //var orderToUpdate = await _orderRepository.GetByIdAsync(request.Id);

            //if (orderToUpdate == null)
            //{
            //    throw new OrderNotFoundException(nameof(Order), request.Id);
            //}
            var updatedEntity = _mapper.Map<Order>(request);
            await _orderRepository.UpdateAsync(updatedEntity);
            _logger.LogInformation("Order {Id} is successfully Updated.", request.Id);
            return Unit.Value;
        }
    }
}
