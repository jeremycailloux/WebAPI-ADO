using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind2API_ADO.Data;
using Northwind2API_ADO.Models;

namespace Northwind2API_ADO.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class SuppliersController : ControllerBase
   {
      private readonly Northwind2Context _context;

      public SuppliersController(Northwind2Context context)
      {
         _context = context;
      }

      [HttpGet]
      public IEnumerable<string> GetPaysFournisseurs()
      {
         return _context.GetCountries();
      }

      [HttpGet("{pays}")]
      public IEnumerable<Supplier> GetFournisseurs(string pays)
      {
         return _context.GetSuppliers(pays);
      }

      [HttpGet("nbproduits")]
      public int GetNbProduits([FromQuery] int id)
      {
         return _context.GetProductsCount(id);
      }
   }
}