using Microsoft.Extensions.Logging;
using Shop.Catalog.Domain.Entities;
using Shop.Catalog.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Shop.Catalog.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IDynamoDbContextFactory _factory;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(IDynamoDbContextFactory factory, ILogger<ProductRepository> logger)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<List<Product>> GetProductsAsync(ProductOptions options)
        {
            List<Product> products;
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"[ProductRepository:GetProductsAsync] ProductOptions:{JsonConvert.SerializeObject(options)}");
            }

            using (Amazon.DynamoDBv2.DataModel.IDynamoDBContext context = _factory.CreateContext())
            {
                // TODO: Add filters - category and product id
                Amazon.DynamoDBv2.DataModel.AsyncSearch<Product> search = context.ScanAsync<Product>(null);
                products = await search.GetNextSetAsync();
            }


            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"[ProductRepository:GetProductsAsync] Product:{JsonConvert.SerializeObject(products)}");
            }


            return products;
        }
    }
}
