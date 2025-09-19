using Catalog.Core.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data.Context
{
    public static class TypeContextSeed
    {
        public static async Task SeedAsync(IMongoCollection<ProductType> typeCollection)
        {
            var hasTypes = await typeCollection.Find(_ => true).AnyAsync();
            if (hasTypes)
            {
                return;
            }
            var filePath = Path.Combine("Data", "SeedData", "types.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file for types not found: {filePath}");
                return;
            }
            var typesData = await File.ReadAllTextAsync(filePath);
            var types = JsonSerializer.Deserialize<IEnumerable<ProductType>>(typesData);
            if (types != null && types.Any())
            {
                await typeCollection.InsertManyAsync(types);
            }
        }
    }
}
