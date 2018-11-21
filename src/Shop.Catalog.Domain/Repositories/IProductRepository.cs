using System;
using System.Collections.Generic;
using System.Text;
using Shop.Catalog.Domain.Entities;

namespace Shop.Catalog.Domain.Repositories
{
    public interface IProductRepository
    {
        List<Product> GetProducts(ProductOptions options);
    }
}
