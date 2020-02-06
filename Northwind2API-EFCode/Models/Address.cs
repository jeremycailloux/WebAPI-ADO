using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_EFCode.Models
{
    public class Address 
    {
        [Key]
        public Guid AddressId { get; set; }
        [Required, MaxLength(100)]
        public string Street { get; set; }
        [Required, MaxLength(40)]
        public string City { get; set; }
        [Required, MaxLength(20)]
        public string PostalCode { get; set; }
        [Required, MaxLength(40)]
        public string Country { get; set; }
        [MaxLength(40)]
        public string Region { get; set; }
        [MaxLength(20)]
        public string  Phone { get; set; }
    }
}
