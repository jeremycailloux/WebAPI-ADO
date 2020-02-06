using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_ADO.Models
{
   public class Supplier
   {
      public int SupplierId { get; set; }
      public string CompanyName { get; set; }
      //public Guid AddressId { get; set; }
      public string HomePage { get; set; }
   }

   public class Address
   {
      public Guid AddressId { get; set; }
      public string Country { get; set; }
   }

   public class Customer
   {
      public string CustomerId { get; set; }
      public string CompanyName { get; set; }
   }

   public class Order
   {
      public int OrderId { get; set; }
      public string CustomerId { get; set; }
      public DateTime OrderDate { get; set; }
      public DateTime? ShippedDate { get; set; }
      public decimal Freight { get; set; }
      public int ItemsCount { get; set; }
      public decimal Total { get; set; }
   }

   public class Product
   {
      public int ProductId { get; set; }
      public string Name { get; set; }
      public Guid CategoryId { get; set; }
      public int SupplierId { get; set; }
      public decimal UnitPrice { get; set; }
      public short UnitsInStock { get; set; }
   }

   public class Category
   {
      public Guid CategoryId { get; set; }
      public string Name { get; set; }
      public string Description { get; set; }
   }

}
