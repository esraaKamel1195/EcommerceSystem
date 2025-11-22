using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.GRPCServices;
using Basket.Application.Responses;
using Basket.Core.Repositories;
using MediatR;

namespace Basket.Application.Handlers.Commands
{
    public class CreateShoppingCartCommandHandler : IRequestHandler<CreateShoppingCartCommand, ShoppingCartResponse>
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly DiscountGrpcService _discountGrpcService;

        public CreateShoppingCartCommandHandler(IBasketRepository basketRepository, IMapper mapper, DiscountGrpcService discountGrpcService) {
            _basketRepository = basketRepository;
            _mapper = mapper;
            _discountGrpcService = discountGrpcService;
        }

        public async Task<ShoppingCartResponse> Handle(CreateShoppingCartCommand request, CancellationToken cancellationToken)
        {
            foreach (var item in request.Items)
            {
                var coupon = await _discountGrpcService.GetDiscount(item.productName);
                if (coupon is not null) {
                    item.price -= coupon.Amount;
                }
            }
            
            var shoppingCart = await _basketRepository.UpdateBasket(new Core.Entities.ShoppingCart()
            {
                UserName = request.UserName,
                Items = request.Items
            });
            ShoppingCartResponse response = _mapper.Map<ShoppingCartResponse>(shoppingCart);
            return response;
        }
    }
}
