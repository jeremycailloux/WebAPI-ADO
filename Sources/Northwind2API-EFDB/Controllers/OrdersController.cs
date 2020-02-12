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
   public class OrdersController : ControllerBase
   {
      private readonly Northwind2Context _context;

      public OrdersController(Northwind2Context context)
      {
         _context = context;
      }

      // GET: api/Orders
      [HttpGet]
      public async Task<ActionResult<IEnumerable<Orders>>> GetOrders([FromQuery] DateTime date1, [FromQuery] DateTime date2)
      {
         if (date2 == DateTime.MinValue) date2 = DateTime.MaxValue; 
         return await _context.Orders.Where(o => o.OrderDate >= date1 && o.OrderDate <= date2).ToListAsync();
      }

      // GET: api/Orders/5
      [HttpGet("{id}")]
      public async Task<ActionResult<Orders>> GetOrders(int id)
      {
         var order = await _context.Orders.Include(o => o.OrderDetail).SingleOrDefaultAsync(o => o.OrderId == id);
         // NB/ Il n'existe pas de méthode FindAsync, il faut donc utiliser SingleOrdDefaultAsync       

         if (order == null)
         {
            return NotFound();
         }

         return order;
      }

      [HttpGet("stats/{year}")]
      public ActionResult<OrdersStats> GetStats(int year)
      {
         OrdersStats stats = new OrdersStats();
         
         // On récupère les commandes de l'année passée en paramètre
         var orders = _context.Orders.Where(o => o.OrderDate.Year == year).Include(o => o.OrderDetail).ToList();

         // On calcule le nombre d'articles commandés et le CA généré par ces commandes
         double revenues = 0d;
         int prodCount = 0;
         foreach (var o in orders)
         {
            prodCount += o.OrderDetail.Sum(od => od.Quantity);
            revenues += o.OrderDetail.Sum(od => (float)od.Quantity * (float)od.UnitPrice * (1.0 - od.Discount));
         }

         stats.OrdersCount = orders.Count(); // Nombre de commandes pour l'année
         stats.ProductsCount = prodCount; // Nombre d'articles commandés
         stats.Revenues = Math.Round(revenues); // CA réalisé

         return stats;
      }


      // PUT: api/Orders/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for
      // more details see https://aka.ms/RazorPagesCRUD.
      [HttpPut("{id}")]
      public async Task<IActionResult> PutOrders(int id, Orders orders)
      {
         if (id != orders.OrderId)
         {
            return BadRequest();
         }

         _context.Entry(orders).State = EntityState.Modified;

         try
         {
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateConcurrencyException)
         {
            if (!OrdersExists(id))
            {
               return NotFound();
            }
            else
            {
               throw;
            }
         }

         return NoContent();
      }

      // POST: api/Orders
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for
      // more details see https://aka.ms/RazorPagesCRUD.
      [HttpPost]
      public async Task<ActionResult<Orders>> PostOrders(Orders orders)
      {
         _context.Orders.Add(orders);
         await _context.SaveChangesAsync();

         return CreatedAtAction("GetOrders", new { id = orders.OrderId }, orders);
      }

      // DELETE: api/Orders/5
      [HttpDelete("{id}")]
      public async Task<ActionResult<Orders>> DeleteOrders(int id)
      {
         // On récupère la commande et ses lignes
         var orders = await _context.Orders.Include(o => o.OrderDetail)
                        .Where(o => o.OrderId == id).SingleOrDefaultAsync();

         if (orders == null)
         {
            return NotFound();
         }

         // On supprime les lignes de commandes
         // car la base ne permet pas la suppression en cascade
         _context.OrderDetail.RemoveRange(orders.OrderDetail);

         // On supprime la commande
         _context.Orders.Remove(orders);

         await _context.SaveChangesAsync();

         return orders;
      }

      private bool OrdersExists(int id)
      {
         return _context.Orders.Any(e => e.OrderId == id);
      }
   }
}
