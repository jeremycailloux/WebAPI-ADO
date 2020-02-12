using System;
using System.Collections.Generic;

namespace Northwind2API_EFDB.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            Orders = new HashSet<Orders>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
    }
}
