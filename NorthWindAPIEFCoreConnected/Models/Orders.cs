using System;
using System.Collections.Generic;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Orders
    {
        public Orders()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public Guid AddressId { get; set; }
        public string CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int ShipperId { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal? Freight { get; set; }
        public string ShipName { get; set; }

        public virtual Address Address { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Employee Employee { get; set; }
        public virtual Shipper Shipper { get; set; }
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
