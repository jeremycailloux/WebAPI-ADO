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
    [Route("[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly Northwind2Context _context;
        public SuppliersController(Northwind2Context context) // Injection de dépendance (du context)
        {
            _context = context;
        }
        // GET: api/Suppliers
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return _context.GetCountries();
        }

        /*
        // GET: api/Suppliers/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        */

        // POST: api/Suppliers
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Suppliers/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // GET: api/GetSuppliers
        [HttpGet("{country}")]
        public List<Supplier> Get(string country)
        {
            return _context.GetSuppliers(country);
        }
        /*
        // GET: api/GetProductsCount
        [HttpGet ("products")] // J'ajoute products à l'URL car sinon l'app ne fait pas la différence avec le GET pour country juste au dessus
        public int Get([FromQuery] int supplierid)
        {
            return _context.GetProductsCount(supplierid);
        }
        */

        // GET: api/GetProductsCount
        [HttpGet("nbproducts")] // J'ajoute products à l'URL car sinon l'app ne fait pas la différence avec le GET pour country juste au dessus
        public int Getnbproducts([FromQuery] string country)
        {
            return _context.GetProductsCount(country);
        }

    }
}
