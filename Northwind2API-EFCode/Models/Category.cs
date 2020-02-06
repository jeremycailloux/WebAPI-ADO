using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_EFCode.Models
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }
        [Required, MaxLength(40)]
        public string Name { get; set; }
        [MaxLength(1000)]
        public string Description { get; set; }

        public virtual List<Product> Products { get; set; }
    }
}
