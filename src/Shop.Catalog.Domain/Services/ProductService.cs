using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Catalog.Domain.Entities;
using Shop.Catalog.Domain.Repositories;

namespace Shop.Catalog.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IProductRepository repository,
            ILogger<ProductService> logger)
        {
            _repository = repository ?? 
                          throw new ArgumentNullException(nameof(repository));
            _logger = logger ??
                      throw new ArgumentNullException(nameof(logger));

        }
        public async Task<List<Product>> GetProductsAsync(ProductOptions options)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"[ProductService:GetProductsAsync] ProductOptions:{JsonConvert.SerializeObject(options)}");
            }

            var products = await _repository.GetProductsAsync(options);


            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"[ProductService:GetProductsAsync] Product:{JsonConvert.SerializeObject(products)}");
            }

            _logger.LogInformation($"Retrieved {products?.Count} products");

            return products;
        }
    }
}
