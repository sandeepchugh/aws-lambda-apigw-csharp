using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Catalog.Domain.Entities
{
    public class Product
    {
        public string ProductId { get; set; }
        public string Category { get; set; }
        public dynamic Data { get; set; }
    }
}
