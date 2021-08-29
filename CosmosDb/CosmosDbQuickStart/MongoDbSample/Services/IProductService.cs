using MongoDbSample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbSample.Services
{
    public interface IProductService
    {
        Task CreateAsync(Product product);
        Task<Product> GetBySkuAsync(string sku);
        Task<List<Product>> GetPagintatedAsync(int page, int itemsPerPage);
        Task<Product> UpdateAsync(Product update);
        Task DeleteAsync(string sku);
    }
}