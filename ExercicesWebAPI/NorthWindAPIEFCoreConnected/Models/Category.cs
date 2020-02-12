using System;
using System.Collections.Generic;

namespace NorthWindAPIEFCoreConnected.Models
{
    public partial class Category
    {
        public Category()
        {
            Product = new HashSet<Product>();
        }

        public Guid CategoryId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public byte[] Picture { get; set; }

        public virtual ICollection<Product> Product { get; set; }
    }
}
