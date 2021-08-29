using Microsoft.AspNetCore.Mvc;
using MongoDbSample.Models;
using MongoDbSample.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MongoDbSample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        IProductService productService;

        public ProductsController(IProductService productService)
        {
            this.productService = productService;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> Get(int page = 1, int itemsPerPage = 10)
        {
            return await productService.GetPagintatedAsync(page, itemsPerPage);
        }

        [HttpGet("{sku}", Name = "GetProduct")]
        public async Task<ActionResult<Product>> GetBySku(string sku)
        {
            var product = await productService.GetBySkuAsync(sku);

            if (product == null)
            {
                return NotFound();
            }

            return product;
        }

        [HttpPost]
        public async Task<ActionResult<Product>> Post(Product product)
        {
            await productService.CreateAsync(product);

            return CreatedAtRoute("GetProduct", new { sku = product.Sku }, product);
        }

        [HttpPut]
        public async Task<ActionResult<Product>> Put(Product update)
        {
            var product = await productService.UpdateAsync(update);

            return CreatedAtRoute("GetProduct", new { sku = product.Sku }, product);
        }

        [HttpDelete("{sku}")]
        public async Task<IActionResult> Delete(string sku)
        {
            await productService.DeleteAsync(sku);

            return Ok();
        }
    }
}
