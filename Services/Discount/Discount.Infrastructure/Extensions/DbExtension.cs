using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discount.Infrastructure.Extensions
{
    public static class DbExtension
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuaration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Discount DB Migration Start");
                    ApplyMigrations(configuaration);
                    logger.LogInformation("Discount Database Migration complete");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Cannot create database migration");
                    throw;
                }
            }
            return host;
        }
        private static void ApplyMigrations(IConfiguration config)
        {
            var retry = 5;
            while (retry > 0)
            {
                try
                {
                    using var connection = new Npgsql.NpgsqlConnection(
                        config.GetValue<string>("DatabaseSettings:ConnectionString")
                        );
                    connection.Open();
                    using var command = new Npgsql.NpgsqlCommand
                    {
                        Connection = connection
                    };
                    command.CommandText = "DROP TABLE IF EXIStS Coupon";
                    command.ExecuteNonQuery();
                    command.CommandText = @"CREATE TABLE Coupon(
                                                    ID SERIAL PRIMARY KEY,
                                                    ProductName VARCHAR(500) NOT NULL,
                                                    Description Text,
                                                    Amount INT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) " +
                        "VALUES ('PowerFit 19 FH Rubber Spike Cricket Shoes'," +
                        "'Designed for professional as well as amateur badminton players. These indoor shoes are crafted with synthetic upper that provides natural fit, while the EVA midsole provides lightweight cushioning. The shoes can be used for Badminton and Squash', 100)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO Coupon(ProductName, Description, Amount) " +
                        "VALUES ('Body Building Egypt Shadow Spirit Mens Badminton Shoes'," +
                        "'Designed for professional as well as amateur badminton players. These indoor shoes are crafted with synthetic upper that provides natural fit, while the EVA midsole provides lightweight cushioning. The shoes can be used for Badminton and Squash', 200)";
                    command.ExecuteNonQuery();
                    break;
                }
                catch (Exception ex)
                {
                    retry--;
                    if (retry == 0)
                    {
                        throw;
                    }
                    Thread.Sleep(2000);
                }
            }
        }
    }
}
