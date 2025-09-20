using Catalog.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Core.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Entities.Product>> GetAllProducts();
        Task<Product> GetProductById(string id);
        Task<IEnumerable<Product>> GetProductByName(string name);
        Task<IEnumerable<Product>> GetProductsByBrandName(string brandName);
        Task<IEnumerable<Product>> GetProductsByTypeName(string typeName);
        Task<IEnumerable<Product>> GetProductsByBrandId(string brandName);
        Task<IEnumerable<Product>> GetProductsByTypeId(string typeName);
        Task<Product> CreateProduct(Product product);
        Task<bool> UpdateProduct(Product product);
        Task<bool> DeleteProduct(string id);
        //object GetAllWithIncludesAsync(List<string> list);
    }
}
