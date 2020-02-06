using System;
using System.Collections.Generic;

namespace Northwind2API_EFDB.Models
{
    public partial class Address
    {
        public Address()
        {
            Supplier = new HashSet<Supplier>();
        }

        public Guid AddressId { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }

        public virtual ICollection<Supplier> Supplier { get; set; }
    }
}
