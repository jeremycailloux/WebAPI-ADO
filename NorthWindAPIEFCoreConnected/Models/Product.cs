using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetail = new HashSet<OrderDetail>();
        }

        [DisplayName("Id")]
        public int ProductId { get; set; }
        [DisplayName("none")]
        public Guid CategoryId { get; set; }
        [DisplayName("Fournisseur")]
        public int SupplierId { get; set; }
        [DisplayName("Nom")]
        public string Name { get; set; }
        [DisplayName("PU")]
        public decimal UnitPrice { get; set; }
        [DisplayName("Quantité")]
        public short UnitsInStock { get; set; }
        [DisplayName("none")]
        public short UnitsOnOrder { get; set; }
        [DisplayName("none")]
        public short ReorderLevel { get; set; }
        [DisplayName("none")]
        public bool Discontinued { get; set; }
        [DisplayName("none")]
        public string QuantityPerUnit { get; set; }
        [DisplayName("none")]
        public byte[] Rowversion { get; set; }

        [DisplayName("none")]
        public virtual Category Category { get; set; }
        [DisplayName("none")]
        public virtual Supplier Supplier { get; set; }
        [DisplayName("none")]
        public virtual ICollection<OrderDetail> OrderDetail { get; set; }
    }
}
