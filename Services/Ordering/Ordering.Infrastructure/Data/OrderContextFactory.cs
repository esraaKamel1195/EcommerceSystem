using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Ordering.Infrastructure.Data
{
    public class OrderContextFactory: IDesignTimeDbContextFactory<OrderContext>
    {
        public OrderContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<OrderContext>();
            optionBuilder.UseSqlServer(
                "Server=localhost;Database=orderDb2;User Id=sa;Password=password@123;TrustServerCertificate=true;");
            return new OrderContext(optionBuilder.Options);
        }
    }
}
