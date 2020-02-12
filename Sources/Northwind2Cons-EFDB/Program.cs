using Microsoft.EntityFrameworkCore;
using Northwind2Cons_EFDB.Models;
using Outils.TConsole;
using System;
using System.Linq;

namespace Northwind2Cons_EFDB
{
   class Program
   {
      private static Northwind2Context _context;
      private static readonly Guid _idCategorie = new Guid("E71DB81C-D2B8-42F0-8F2C-1A901BDE824F");

      static void Main(string[] args)
      {
         _context = new Northwind2Context();
         _context.Product.Where(p => p.CategoryId == _idCategorie).Load();
         DisplayProductsAndMenu();

         Console.ReadKey();
      }

      private static void DisplayProductsAndMenu()
      {
         Console.Clear();
         DisplayProducts();
         DisplayMenu();
      }

      private static void DisplayMenu()
      {
         Console.WriteLine();
         Console.WriteLine("1. Ajouter un produit");
         Console.WriteLine("2. Modifier un produit");
         Console.WriteLine("3. Supprimer un produit");
         Console.WriteLine("4. Enregistrer");
         Console.WriteLine("5. Annuler");
         Console.WriteLine("Choix : ");

         int choix;
         while (!int.TryParse(Console.ReadLine(), out choix) || choix < 1 || choix > 5);


         switch (choix)
         {
            case 1:
               CreateProduct();
               break;
            case 2:
               UpdateProduct();
               break;
            case 3:
               DeleteProduct();
               break;
            case 4:
               SaveChanges();
               break;
            case 5:
               CancelChanges();
               break;
            default:
               break;
         }
      }

      private static void DisplayProducts()
      {
         var prods = _context.Product.Local.ToList();
         ConsoleTable.From(prods).Display("produits");
      }

      private static void CreateProduct()
      {
         Product p = new Product();

         Console.WriteLine("Nom :");
         p.Name = Console.ReadLine();

         Console.WriteLine("Fournisseur :");
         p.SupplierId = int.Parse(Console.ReadLine());

         Console.WriteLine("PU :");
         p.UnitPrice = decimal.Parse(Console.ReadLine());

         Console.WriteLine("Unités en stock :");
         p.UnitsInStock = short.Parse(Console.ReadLine());

         p.CategoryId = _idCategorie;

         _context.Product.Add(p);

         DisplayProductsAndMenu();
      }

      private static void UpdateProduct()
      {
         Console.WriteLine("Id du produit à modifier :");
         int id = int.Parse(Console.ReadLine());

         Product p = _context.Product.Find(id);
         Console.WriteLine($"{p.Name}, fourni par {p.SupplierId}," +
            $" {p.UnitPrice:C2}, {p.UnitsInStock} en stock");

         Console.WriteLine("Nouveau nom :");
         p.Name = Console.ReadLine();

         Console.WriteLine("Nouveau fournisseur :");
         p.SupplierId = int.Parse(Console.ReadLine());

         Console.WriteLine("Nouveau PU :");
         p.UnitPrice = decimal.Parse(Console.ReadLine());

         Console.WriteLine("Unités en stock :");
         p.UnitsInStock = short.Parse(Console.ReadLine());

         //_context.Product.Update(p); // pas nécessaire en mode connecté
         DisplayProductsAndMenu();
      }

      private static void DeleteProduct()
      {
         Console.WriteLine("Id du produit à supprimer :");
         int id = int.Parse(Console.ReadLine());

         Product p = _context.Product.Find(id);
         Console.WriteLine($"{p.Name}, fourni par {p.SupplierId}," +
            $" {p.UnitPrice:C2}, {p.UnitsInStock} en stock");

         _context.Product.Remove(p);
         DisplayProductsAndMenu();
      }

      private static void SaveChanges()
      {
         try
         {
            _context.SaveChanges();
         }
         catch (DbUpdateConcurrencyException)
         {
            Console.WriteLine("Le produit a été modifié entre temps. Rafraîchissez vos données avant d'enregistrer");
            Console.ReadKey();
         }

         DisplayProductsAndMenu();
      }

      private static void CancelChanges()
      {
         foreach (var entry in _context.ChangeTracker.Entries<Product>())
         {
            switch (entry.State)
            {
               case EntityState.Modified:
               case EntityState.Deleted:
                  // On annule d'abord les modifications de l’entité supprimée
                  entry.State = EntityState.Modified;
                  entry.State = EntityState.Unchanged;
                  break;
               case EntityState.Added:
                  entry.State = EntityState.Detached;
                  break;
            }
         }

         DisplayProductsAndMenu();
      }
   }
}
