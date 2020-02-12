using System;
using System.Collections.Generic;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Shipper
    {
        public Shipper()
        {
            Orders = new HashSet<Orders>();
            Shipping = new HashSet<Shipping>();
        }

        public int ShipperId { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Orders> Orders { get; set; }
        public virtual ICollection<Shipping> Shipping { get; set; }
    }
}
