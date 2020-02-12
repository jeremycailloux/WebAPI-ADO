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
            _context = context; // Injection de dépendance (du context)
        }

        // GET: api/Products/5
        /*        [HttpGet("{id}", Name = "Get")]
                public string Get(int id)
                {
                    return "value";
                }

        */

        // GET: api/Products?search=
        [HttpGet]
        [Produces(typeof(Product))]
        public IActionResult Getpro([FromQuery]string search)
        {
            if (string.IsNullOrEmpty(search) || search.Length < 3) return BadRequest();
            var products = _context.FindProducts(search);
            if (/*products.Count == 0*/ !products.Any()) return NotFound(); // !products.Any est mieux
            return Ok(products);
        }

        // GET: api/Products/productid
        [HttpGet("{productid}", Name = "GetProduct")] // On fait référence à la méthode GetProduct
        [Produces(typeof(Product))]
        public IActionResult Getpro(int productId)
        {
            var product = _context.GetProduct(productId);
            if (product == null) return NotFound();
            return Ok(product); // Ou else return Ok(product)
        }

        // POST: api/Products
        [HttpPost] // Creation d'un nouveau produit
        public IActionResult Post([FromBody] Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name) || product.CategoryId == null || product.SupplierId == 0)
                return BadRequest();
            try
            {
                int productid = _context.CreateProduct(product);
                return CreatedAtAction(nameof(Getpro), new { id = productid }, product);
            }

            catch (SqlException ex)
            {
                return SendError(ex); // Gestion de l'erreur
            }
        }

        // PUT: api/Products/5 
        [HttpPut]  //Mise à jour d’un produit
        public IActionResult Put([FromQuery] int productid, [FromBody] Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Name) || product.CategoryId == null || product.SupplierId == 0 || product.ProductId != productid || productid == 0)
            return BadRequest();

            try //Gestion de l'erreur
            {
                _context.UpdateProduct(product);
                return Ok(); // Ou return NoContent();
            }

            catch (SqlException ex)
            {
                return SendError(ex);
            }
        }

        // DELETE: api/Products
        [HttpDelete("{productid}")]
        public IActionResult Delete([FromRoute] int productid)
        {
            try
            {
                int nblignes = _context.DeleteProduct(productid);
                if (nblignes == 0) return NotFound();

                return NoContent();
            }

            catch (SqlException ex)
            {
                return SendError(ex);
            }
        }

        private ActionResult SendError(SqlException ex) // Cette méthode n'est pas décorée par un attribut (Get, Post, Put, Delete), elle n'est donc pas reconnue en tant qu'action
        {
            if (ex.Number == 547)
            {
                if (ex.Message.Contains("Product_Category_FK"))
                    return BadRequest("La catégorie spécifiée pour ce produit n'éxiste pas");
                if (ex.Message.Contains("Product_Supplier_FK"))
                    return BadRequest("Le fournisseur spécifié n'existe pas");
                if (ex.Message.Contains("OrderDetail_Product_FK"))
                    return BadRequest("Ce produit ne peut pas être supprimé car il est référencé dans une commande");
            }
            // Pour toutes les autres erreurs, on renvoie une réponse de code http 500
            // avec un corps contenant le message de l'erreur
            return StatusCode(500, ex.Message);
        }
    }
}
