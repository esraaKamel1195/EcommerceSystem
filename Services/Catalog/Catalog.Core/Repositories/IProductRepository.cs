using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Core.Repositories
{
    public interface IProductRepository
    {
        Task<Pagination<Product>> GetAllProducts(CatalogSpecParams catalogSpecParams);
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
