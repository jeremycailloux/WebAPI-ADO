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

            if (date1 == DateTime.MinValue && date2 == DateTime.MinValue)
            {
                return BadRequest();
            }

            if (date1 == DateTime.MinValue)
            {
                return await _context.Orders.Where(o => o.OrderDate <= date2).OrderBy(o => o.OrderDate).ToListAsync();
            }

            else if (date2 == DateTime.MinValue)
            {
                return await _context.Orders.Where(o => o.OrderDate >= date1).OrderBy(o => o.OrderDate).ToListAsync();
            }

            /* ou bien
             if(date2 == DateTime.MinValue)
            {
                date2 = DateTime.MaxValue;
            }
            */

            return await _context.Orders.Where(o => (o.OrderDate >= date1) && (o.OrderDate <= date2)).OrderBy(o => o.OrderDate).ToListAsync();
        }

        /*
        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orders>> GetOrders(int id)
        {
            var orders = await _context.Orders.FindAsync(id);

            if (orders == null)
            {
                return NotFound();
            }

            return orders;
        }
        */

        // GET: api/Orders/5
        [HttpGet("{customerid}")]
        public async Task<ActionResult<IEnumerable<Orders>>> GetOrdersOfCustomer(string customerid)
        {
            var orders = await _context.Orders.Where(o => o.CustomerId == customerid).ToListAsync();

            if (orders == null)
            {
                return NotFound();
            }

            return orders;
        }
        
        /*
        // GET: api/Orders/orderdetail/5
        [HttpGet("orderdetail/{id}")]
        public async Task<ActionResult<Orders>> GetOrders(int id)
        {
            var orders = await _context.Orders.Include(o => o.OrderDetail).SingleOrDefaultAsync(o => o.OrderId == id);

            if (orders == null)
            {
                return NotFound();
            }

            return orders;
        }
        */

        //GET: api/Orders/stats/2015
        [HttpGet("stats/{year}")]
        public async Task<ActionResult<OrdersStats>> GetOrders(int year)
        {
            var ordersOfYear = await _context.Orders.Include(o => o.OrderDetail).Where(o => o.OrderDate.Year == year).ToListAsync();
            var orderStats = new OrdersStats
            {
                OrdersCount = ordersOfYear.Count(),
                ProductsCount = 0,
                Revenues = 0
            };

            foreach (var order in ordersOfYear)
            {
                orderStats.ProductsCount += order.OrderDetail.Sum(od => od.Quantity);
                orderStats.Revenues += order.OrderDetail.Sum(od => od.Quantity * od.UnitPrice * (1 - (decimal)od.Discount));
            }

            return orderStats;
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
            // Quand on supprime une commande, supprimer également les lignes de commandes associées.
            var orders = await _context.Orders.Include(o => o.OrderDetail).FirstOrDefaultAsync(o => o.OrderId == id);
            if (orders == null)
            {
                return NotFound();
            }
            
            _context.OrderDetail.RemoveRange(orders.OrderDetail);
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
