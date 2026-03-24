using Microsoft.EntityFrameworkCore;
using Ordering.Core.Entities;
using Ordering.Core.Repositories;
using Ordering.Infrastructure.Data;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository: RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(OrderContext dbContext) :base(dbContext) { }


        public async Task<IEnumerable<Order>> GetOrdersByUsername(string username)
        {
            var ordersList = await _orderContext.Orders
                .Where(e => e.UserName == username).ToListAsync();
            return ordersList;
        }
    }
}
