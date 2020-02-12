using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Northwind2API_EFDB.Models;

namespace Northwind2API_EFDB.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class CustomersController : ControllerBase
   {
      private readonly Northwind2Context _context;

      public CustomersController(Northwind2Context context)
      {
         _context = context;
      }

      // GET: api/Customers
      [HttpGet]
      public async Task<ActionResult<IEnumerable<Customer>>> GetCustomer()
      {
         return await _context.Customer.ToListAsync();
      }

      // GET: api/Customers/5
      [HttpGet("{id}")]
      public async Task<ActionResult<Customer>> GetCustomer(string id)
      {
         var customer = await _context.Customer.FindAsync(id);

         if (customer == null)
         {
            return NotFound();
         }

         return customer;
      }

      // PUT: api/Customers/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for
      // more details see https://aka.ms/RazorPagesCRUD.
      [HttpPut("{id}")]
      public async Task<IActionResult> PutCustomer(string id, Customer customer)
      {
         if (id.ToLower() != customer.CustomerId.ToLower())
         {
            return BadRequest();
         }

         //_context.Entry(customer).State = EntityState.Modified;
         _context.Attach(customer);
         _context.Entry(customer).Property(nameof(customer.ContactName)).IsModified = true;
         _context.Entry(customer).Property(nameof(customer.ContactTitle)).IsModified = true;

         try
         {
            await _context.SaveChangesAsync();
            await _context.Entry<Customer>(customer).ReloadAsync(); // Force à recharger l'entité de la base pour récupérer les propriétés non mises à jour
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!CustomerExists(id))
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return Ok(_context.Customer.FindAsync(id));
      }

      // POST: api/Customers
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for
      // more details see https://aka.ms/RazorPagesCRUD.
      [HttpPost]
      public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
      {
         _context.Customer.Add(customer);
         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateException)
         {
            if (CustomerExists(customer.CustomerId))
            {
               return Conflict();
            }
            else
            {
               throw;
            }
         }

         return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
      }

      // DELETE: api/Customers/5
      [HttpDelete("{id}")]
      public async Task<ActionResult<Customer>> DeleteCustomer(string id)
      {
         var customer = await _context.Customer.FindAsync(id);
         if (customer == null)
         {
            return NotFound();
         }

         _context.Customer.Remove(customer);
         await _context.SaveChangesAsync();

         return customer;
      }

      private bool CustomerExists(string id)
      {
         return _context.Customer.Any(e => e.CustomerId == id);
      }
   }
}
