using System;
using System.Collections.Generic;

namespace Northwind2API_EFDB.Models
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public short Quantity { get; set; }
        public float Discount { get; set; }

        internal virtual Orders Order { get; set; }
        internal virtual Product Product { get; set; }
    }
}
