using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_EFCode.Models
{
    public class Supplier
    {
        [Key]
        public Guid SupplierId { get; set; }

        [ForeignKey("Address")]
        public int AddressId { get; set; }
        [Required, MaxLength(100)]
        public string CompanyName { get; set; }
        [MaxLength(100)]
        public string ContactName { get; set; }
        [MaxLength(40)]
        public string ContactTitle { get; set; }
        [MaxLength(100)]
        public string HomePage { get; set; }
        public virtual List<Product> products { get; set; }
        public virtual Address Address { get; set; }
    }
}
