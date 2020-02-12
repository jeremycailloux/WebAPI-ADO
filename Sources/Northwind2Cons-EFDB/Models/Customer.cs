using System;
using System.Collections.Generic;

namespace Northwind2Cons_EFDB.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Orders>();
        }

        public string CustomerId { get; set; }
        public Guid AddressId { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }

        public virtual Address Address { get; set; }
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
