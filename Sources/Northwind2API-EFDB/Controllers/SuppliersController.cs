using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Northwind2API_EFDB.Models;

namespace Northwind2API_EFDB.Controllers
{
   [Route("Northwind2/[controller]")]
   [ApiController]
   public class SuppliersController : ControllerBase
   {
      private readonly Northwind2Context _context;

      public SuppliersController(Northwind2Context context)
      {
         _context = context;
      }

      // GET: api/Suppliers
      [HttpGet]
      public async Task<ActionResult<IEnumerable<Supplier>>> GetSupplier()
      {
         return await _context.Supplier.ToListAsync();
      }

      // GET: api/Suppliers/5
      [HttpGet("{id}")]
      public async Task<ActionResult<Supplier>> GetSupplier(int id)
      {
         // Recheche selon la clé primaire
         var supplier = await _context.Supplier.FindAsync(id);

         if (supplier == null)
         {
            return NotFound();
         }

         return supplier;
      }


      // POST: api/Suppliers
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for
      // more details see https://aka.ms/RazorPagesCRUD.
      [HttpPost]
      public async Task<ActionResult<Supplier>> PostSupplier(Supplier supplier)
      {
         _context.Supplier.Add(supplier);
         await _context.SaveChangesAsync();

         return CreatedAtAction("GetSupplier", new { id = supplier.SupplierId }, supplier);
      }

      // PUT: api/Suppliers/5
      // To protect from overposting attacks, please enable the specific properties you want to bind to, for
      // more details see https://aka.ms/RazorPagesCRUD.
      [HttpPut("{id}")]
      public async Task<IActionResult> PutSupplier(int id, Supplier supplier)
      {
         if (id != supplier.SupplierId)
         {
            return BadRequest("L’identifiant du fournisseur ne correspond pas à celui passé en premier paramètre");
         }

         _context.Entry(supplier).State = EntityState.Modified;

         bool saved = false;
         while (!saved)
         {
            try
            {
               await _context.SaveChangesAsync();
               saved = true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
               if (!SupplierExists(id))
                  return NotFound();
               else
               {
                  // Si on ne peut pas résoudre le conflit, on renvoie une réponse Conflict
                  // L'appli cliente doit inviter l'utilisateur à rafraîchir ses données
                  // avant de tenter une nouvelle mise à jour
                  if (!TryResolveConflict(ex))
                     return Conflict("User should refresh data before posting updates");
               }
            }
         }

         return NoContent();
      }

      /// <summary>
      /// Essaie de résoudre les conflits d'accès concurrentiels
      /// (inspiré de cette page de doc : https://docs.microsoft.com/fr-fr/ef/core/saving/concurrency)
      /// </summary>
      /// <param name="ex">Exception contenant les infos nécessaires au traitement du conflit </param>
      /// <returns>True si les conflits ont été résolus automatiquement, False sinon (dans ce cas, il faut
      /// inviter l'utilisateur à rafraichir ses données avant qu'il ne renvoie ses modifications</returns>
      private bool TryResolveConflict(DbUpdateConcurrencyException ex)
      {
         // NB/ Dans les faits, il n'y a qu'une seule entité à vérifier bien qu'elle soit transmise dans une collection
         foreach (var entry in ex.Entries)
         {
            // On contrôle le type de l'entité
            Supplier s = entry.Entity as Supplier;
            if (!(entry.Entity is Supplier))
               throw new NotSupportedException("Unable to solve conflict on " + entry.Metadata.Name);

            // On récupère les valeurs des propriétés de l'entité
            // (celles proposées par l'utilisateur et celles issues de la base)
            var proposedValues = entry.CurrentValues;
            var databaseValues = entry.GetDatabaseValues();

            // On recherche la propriété définie comme jeton d'accès concurrentiel
            foreach (var prop in proposedValues.Properties)
            {               
               if (prop.IsConcurrencyToken)
               {
                  // On récupère la valeur envoyée par l'utilisateur et celle présente en base
                  var propValue = proposedValues[prop];
                  var dbValue = databaseValues[prop];  
                  
                  // Si la valeur proposée est vide alors que celle en base ne l'est pas
                  // on interrompt le traitement, pour laisser l'utilisateur résoudre le conflit
                  if (string.IsNullOrEmpty((string)propValue) && !string.IsNullOrEmpty((string)dbValue))
                  {
                     return false;
                  }
               }
            }

            // Pour les autres cas de figure non problématiques, on affecte les valeurs
            // les valeurs originales de l'entrée pour shunter la prochaine vérification
            // d'accès concurrentiel, afin que l'exception ne soit plus émise à la prochaine
            // tentative d'enregistrement
            entry.OriginalValues.SetValues(databaseValues);
         }
         return true;
      }

      // DELETE: api/Suppliers/5
      [HttpDelete("{id}")]
      public async Task<ActionResult<Supplier>> DeleteSupplier(int id)
      {
         var supplier = await _context.Supplier.Include(s => s.Address)
                        .SingleOrDefaultAsync(s => s.SupplierId == id);
         if (supplier == null)
            return NotFound();

         // Si l'enregistrement des modifs échoue c'est que le fournisseur ne peut pas être supprimé
         try
         {
            _context.Supplier.Remove(supplier);
            _context.Address.Remove(supplier.Address);
            await _context.SaveChangesAsync();
         }
         catch (DbUpdateException)
         {
            return BadRequest($"Le fournisseur {id} ne peut pas être supprimé, car il est utilisé");
         }         

         return Ok(supplier);
      }

      private bool SupplierExists(int id)
      {
         return _context.Supplier.Any(e => e.SupplierId == id);
      }
   }
}
