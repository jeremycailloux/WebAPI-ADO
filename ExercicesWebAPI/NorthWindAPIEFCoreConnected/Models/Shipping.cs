using System;
using System.Collections.Generic;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Shipping
    {
        public int ShippingId { get; set; }
        public DateTime DeliveryDate { get; set; }
        public DateTime DeliveryHour { get; set; }
        public int? QuantityOfProducts { get; set; }
        public int NewShipperId { get; set; }

        public virtual Shipper NewShipper { get; set; }
    }
}
