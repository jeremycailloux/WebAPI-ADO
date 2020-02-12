using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoList.Models;
using TodoListAPI.Data;

namespace TodoListAPI.Controllers
{
   [Route("api/[controller]")]
   [ApiController]
   public class TodoListController : ControllerBase
   {
      private readonly ITodoListContext _context;

      public TodoListController(ITodoListContext context)
      {
         _context = context;
      }

      // GET api/TodoList
      [HttpGet]
      public ActionResult<IEnumerable<Tache>> Get()
      {
         var taches = _context.GetTaches();
         return taches;
      }

      // GET api/TodoList/5
      [HttpGet("{id}")]
      public ActionResult<Tache> Get(int id)
      {
         var taches = _context.GetTaches();
         return taches.Where(t => t.Id == id).FirstOrDefault();
      }

      // POST api/TodoList
      [HttpPost]
      public void Post([FromBody] Tache tache)
      {
         _context.AjouterTache(tache);
      }

      // PUT api/TodoList/5
      [HttpPut("{id}")]
      public void Put(int id, [FromBody] Tache tache)
      {
         _context.ModifierTache(tache);
      }

      // DELETE api/TodoList/5
      [HttpDelete("{id}")]
      public void Delete(int id)
      {
         _context.SupprimerTache(id);
      }
   }
}
