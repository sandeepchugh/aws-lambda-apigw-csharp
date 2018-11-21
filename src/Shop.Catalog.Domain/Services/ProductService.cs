using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Shop.Catalog.Domain.Entities;
using Shop.Catalog.Domain.Repositories;

namespace Shop.Catalog.Domain.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;
        private readonly ILogger<IProductRepository> _logger;

        public ProductService(IProductRepository repository,
            ILogger<IProductRepository> logger)
        {
            _repository = repository ?? 
                          throw new ArgumentNullException(nameof(repository));
            _logger = logger ??
                      throw new ArgumentNullException(nameof(logger));

        }
        public List<Product> GetProducts(ProductOptions options)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"[ProductService:GetProducts] ProductOptions:{JsonConvert.SerializeObject(options)}");
            }

            var products = _repository.GetProducts(options);


            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug($"[ProductService:GetProducts] Product:{JsonConvert.SerializeObject(products)}");
            }

            _logger.LogInformation($"Retrieved {products?.Count} products");

            return products;
        }
    }
}
