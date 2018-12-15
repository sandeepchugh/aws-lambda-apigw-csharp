using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Catalog.Domain.Entities
{
    public class Product
    {
        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public decimal ListPrice { get; set; }
        public decimal SalePrice { get; set; }
        public int DiscountPercent { get; set; }
    }
}
