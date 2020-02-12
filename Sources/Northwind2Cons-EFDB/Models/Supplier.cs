using System;
using System.Collections.Generic;

namespace Northwind2Cons_EFDB.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Product = new HashSet<Product>();
        }

        public int SupplierId { get; set; }
        public Guid AddressId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string HomePage { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Product> Product { get; set; }
    }
}
