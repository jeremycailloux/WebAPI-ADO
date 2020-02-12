using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoListApi.Models;
using TodoListApi.Data;

namespace TodoListApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TodoListController : ControllerBase
    {

        private readonly TodoListMemContext _context;

        public TodoListController(TodoListMemContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IEnumerable<Tache> Get()
        {
            return _context.GetTaches();
        }

        [HttpGet("{id}")]
        public Tache Get(int id)
        {
            return _context.GetTaches()[id];
        }

        [HttpPost]
        public void Post([FromBody] Tache tache)
        {
            _context.AjouterTache(tache);
        }

        [HttpPut("{id}")]
        public void Put([FromRoute] int id, [FromBody] Tache tache)
        {
            _context.ModifierTache(id, tache);
        }

        [HttpDelete("{id}")]

        public void Delete([FromRoute] int id)
        {
            _context.SupprimerTache(id);
        }
    }
}
