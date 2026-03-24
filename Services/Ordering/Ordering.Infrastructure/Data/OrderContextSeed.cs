using Microsoft.Extensions.Logging;
using Ordering.Core.Entities;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextSeed
    {
        public static async Task SeedAsync(OrderContext orderContext, ILogger<OrderContextSeed> logger)
        {
            if (!orderContext.Orders.Any())
            {
                orderContext.Orders.AddRange(GetOrders());
                await orderContext.SaveChangesAsync();
                logger.LogInformation("Seed data is successfully added to the database");
            }
        }

        private static IEnumerable<Order> GetOrders()
        {
            return new List<Order> {
                    new Order {
                        UserName = "swn",
                        FirstName = "Sina",
                        LastName = "Wang",
                        EmailAddress = "test@email.com",
                        AddressLine = "1st Street",
                        Country = "China",
                        TotalPrice = "100",
                        State ="trsst",
                        ZibCode ="15263",
                        CardName = "tes test",
                        CardNumber = "12344567891252",
                        Expiration = "20/30",
                        CVV = "321",
                        PaymentMethod = "Cash",
                    }
            };
        }
    }
}
