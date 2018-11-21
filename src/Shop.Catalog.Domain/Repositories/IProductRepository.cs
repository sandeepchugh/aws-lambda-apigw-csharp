using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Shop.Catalog.Domain.Entities;

namespace Shop.Catalog.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync(ProductOptions options);
    }
}
