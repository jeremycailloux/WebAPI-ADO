using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_ADO.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public Guid CategoryId { get; set; }
        public int SupplierId { get; set; }
        public string Name { get; set; }
        public decimal UnitPrice { get; set; }
        public short UnitsInStock { get; set; }
    }
}
