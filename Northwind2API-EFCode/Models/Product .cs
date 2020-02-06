using Northwind2API_EFCode.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_EFCode
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }
        [ForeignKey("Supplier")]
        public int CategoryId { get; set; }
        [ForeignKey("Address")]
        public int SupplierId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        [Required, Column(TypeName = "Money")]
        public decimal UnitPrice { get; set; }
        [Required]
        public short UnitsInStock { get; set; }
        [Required]
        public short UnitsOnOrder { get; set; }

        public virtual Supplier Supplier { get; set; }
        public virtual Category Category { get; set; }
    }
}
