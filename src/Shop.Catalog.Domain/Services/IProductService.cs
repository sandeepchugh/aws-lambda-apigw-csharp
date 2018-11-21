using System;
using System.Collections.Generic;
using System.Text;
using Shop.Catalog.Domain.Entities;

namespace Shop.Catalog.Domain.Services
{
    public interface IProductService
    {
        List<Product> GetProducts(ProductOptions options);
    }
}
