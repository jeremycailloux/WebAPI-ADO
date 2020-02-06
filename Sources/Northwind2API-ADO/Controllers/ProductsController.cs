using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Northwind2API_ADO.Data;
using Northwind2API_ADO.Models;

namespace Northwind2API_ADO.Controllers
{
   [Route("[controller]")]
   [ApiController]
   public class ProductsController : ControllerBase
   {
      private readonly Northwind2Context _context;

      public ProductsController(Northwind2Context context)
      {
         _context = context;
      }

      // GET: products?namecontains=rie
      [HttpGet]
      public ActionResult<List<Product>> FindProducts(string nameContains)
      {
         // Si le texte recherché contient moins de 3 caractères
         // on considère la requête non valide
         if (string.IsNullOrWhiteSpace(nameContains) || nameContains.Length < 3)
            return BadRequest();

         // On récupère les produits dont le nom contient le texte recherché
         var products = _context.FindProducts(nameContains);

         // Si aucun aliment n'a été trouvé, on renvoie une répone NotFound
         if (!products.Any()) return NotFound();

         // On renvoie la liste des produits trouvés
         return Ok(products);
      }


      // GET: products/77
      // Pour qu'il n'y ait pas de conflit d'url avec l'action précédente,
      // on passe l'id dans un segment d'url
      [HttpGet("{id}", Name = "GetProduct")]
      public ActionResult<Product> GetProduct(int id)
      {
         var product = _context.GetProduct(id);
         if (product == null) return NotFound();

         return Ok(product);
      }

      // POST: Products
      [HttpPost]
      public ActionResult<Product> Post([FromBody] Product prod)
      {
         if (string.IsNullOrEmpty(prod.Name) || prod.CategoryId == Guid.Empty || prod.SupplierId == 0)
            return BadRequest();

         // on crée le produit dans la base et on récupère son Id
         try
         {
            prod.ProductId = _context.CreateProduct(prod);

            // On renvoie une réponse avec les en-têtes "status Code: 201 Created"
            // et "location: <url d'accès au produit>", et un corps contenant le produit lui-même
            return CreatedAtAction(nameof(GetProduct), new { id = prod.ProductId }, prod);
         }
         catch (SqlException ex)
         {
            return SendError(ex);
         }

      }

      // PUT: Products/78
      [HttpPut("{id}")]
      public ActionResult Put(int id, [FromBody] Product prod)
      {
         // On vérifie que le produit à un Id identique à celui passé en paramètre
         if (string.IsNullOrEmpty(prod.Name) || id <= 0 || prod.ProductId != id ||
            prod.CategoryId == Guid.Empty || prod.SupplierId == 0)
            return BadRequest();

         try
         {
            // On met à jour le produit
            _context.UpdateProduct(prod);

            // On renvoie une réponse avec l'en-tête "status Code: 200 Ok" et un corps vide
            return Ok();
         }
         catch (SqlException ex)
         {
            return SendError(ex);
         }
      }

      // DELETE: Products/78
      [HttpDelete("{id}")]
      public ActionResult Delete(int id)
      {
         try
         {
            // On supprime le produit
            int res = _context.DeleteProduct(id);
            // S'il existait bien dans la base, on revoie une réponse avec l'en-tête "Status Code : 204"
            if (res > 0) return NoContent();
            return NotFound();
         }
         catch (SqlException ex)
         {
            return SendError(ex);
         }         
      }

      private ActionResult SendError(SqlException ex)
      {
         if (ex.Number == 547)
         {
            if (ex.Message.Contains("Product_Category_FK"))
               return BadRequest("La catégorie spécifiée pour ce produit n'existe pas");

            if (ex.Message.Contains("Product_Supplier_FK"))
               return BadRequest("Le fournisseur spécifié pour ce produit n'existe pas");

            if (ex.Message.Contains("OrderDetail_Product_FK"))
               return BadRequest("Ce produit ne peut pas être supprimé car il est référencé dans une commande");
         }
         
         // Pour toutes les autres erreurs, on renvoie une réponse de code http 500
         // avec un corps contenant le message de l'erreur
         return StatusCode(500, ex.Message);
      }
   }
}
