using MongoDB.Driver;
using MongoDbSample.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MongoDbSample.Services
{
    public class ProductService : IProductService
    {
        private IMongoDatabase db;
        private IMongoCollection<Product> products;

        public ProductService(IMongoClient client)
        {
            db = client.GetDatabase("cosmicworks");
            products = db.GetCollection<Product>("products");
        }

        public Task<List<Product>> GetPagintatedAsync(int page, int itemsPerPage)
        {
            var skipAmount = (page - 1) * itemsPerPage;

            return products.Find(product => true)
                .Skip(skipAmount)
                .Limit(itemsPerPage)
                .ToListAsync();
        }

        public Task<Product> GetBySkuAsync(string sku)
            => products.Find(p => p.Sku == sku).FirstOrDefaultAsync();

        public Task CreateAsync(Product product)
            => products.InsertOneAsync(product);

        public Task<Product> UpdateAsync(Product update)
        {
            var filter = Builders<Product>.Filter.Eq(p => p.Sku, update.Sku);
            var findAndReplaceOptions = new FindOneAndReplaceOptions<Product> { ReturnDocument = ReturnDocument.After };
            return products.FindOneAndReplaceAsync(filter, update, findAndReplaceOptions);
        }

        public Task DeleteAsync(string sku) 
            => products.DeleteOneAsync(p => p.Sku == sku);
    }
}
