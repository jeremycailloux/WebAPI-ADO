using Microsoft.EntityFrameworkCore;
using NorthWindAPIEFCoreConnected.Models;
using Outils.TConsole;
using System;
using System.Linq;

namespace NorthWindAPIEFCoreConnected
{
    class Program
    {
        //	Instancier un contexte de données et le mémoriser dans un champ privé statique pour qu’il soit accessible par les autres méthodes de la classe
        private static Northwind2Context _context = new Northwind2Context(); // Instancier un contexte de données et le mémoriser dans un champ privé statique pour qu’il soit accessible par les autres méthodes de la classe
        private static Guid _idCategory = new Guid("e71db81c-d2b8-42f0-8f2c-1a901bde824f"); // id de la catégorie Melt/Poultry stoquée en dur dans le code dans un champ privé

        static void Main(string[] args)
        {
            // Charger le DbSet des produits avec les produits de la catégorie « Meat/Poultry » dont on stockera l’id en dur dans le code, dans un champ privé.
            _context.Product.Where(p => p.CategoryId == _idCategory).Load(); // A partir du moment où on a fait le Load le DbSet est chargé

            DisplayProductsAndMenu();
            Console.ReadKey();

        }

        // Création des méthodes d’affichage du menu et de la liste
        private static void DisplayMenu()
        {
            // Affiche le menu suivant et appelle la méthode correspondant au choix de l’utilisateur
            Console.WriteLine();
            Console.WriteLine("1. Ajouter un produit");
            Console.WriteLine("2. Modifier un produit");
            Console.WriteLine("3. Supprimer un produit");
            Console.WriteLine("4. Enregistrer");
            Console.WriteLine("5. Annuler");

            int choix;
            while (!int.TryParse(Console.ReadLine(), out choix) || choix < 1 || choix > 5) ;

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
            }
        }

        static void DisplayProducts()
        {
            // Affiche sous forme de tableau le contenu de la vue locale
            var prods = _context.Product.Local.ToList(); //Local permettra d'afficher les modifications que l'on va apporter
            ConsoleTable.From(prods).Display("produits"); // Dans la méthode From on récupère la liste des produits dans le DbSet
            // Affiche sous forme de tableau le contenu de la vue locale
            // « prods » est le nom de la liste contenant les données de la vue locale.
            // « produits » est le libellé qui s’affiche au-dessus du tableau

        }

        public static void DisplayProductsAndMenu()
        {
            Console.Clear();
            DisplayProducts();
            DisplayMenu();
            // Vide l‘écran, puis appelle les méthodes DisplayProducts et DisplayMenu  
        }

        public static void CreateProduct()
        {
            var product = new Product();

            Console.WriteLine("Veuillez saisir l'identifiant du fournisseur");
            product.SupplierId = int.Parse(Console.ReadLine());

            Console.WriteLine("Veuillez saisir le nom du produit");
            product.Name = Console.ReadLine();

            Console.WriteLine("Veuillez saisir le prix unitaire du produit");
            product.UnitPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Veuillez saisir la quantité en stock");
            product.UnitsInStock = short.Parse(Console.ReadLine());

            product.CategoryId = _idCategory;

            _context.Product.Add(product);

            DisplayProductsAndMenu();
        }

        public static void UpdateProduct()
        {

            Console.WriteLine("Veuillez saisir l'Id du produit à modifier");
            int id = int.Parse(Console.ReadLine());

            Product product = _context.Product.Find(id);

            Console.WriteLine("Veuillez saisir l'identifiant du fournisseur");
            product.SupplierId = int.Parse(Console.ReadLine());

            Console.WriteLine("Veuillez saisir le nom du produit");
            product.Name = Console.ReadLine();

            Console.WriteLine("Veuillez saisir le prix unitaire du produit");
            product.UnitPrice = decimal.Parse(Console.ReadLine());

            Console.WriteLine("Veuillez saisir la quantité en stock");
            product.UnitsInStock = short.Parse(Console.ReadLine());

            product.CategoryId = _idCategory;

            // _context.Product.Update(product); // Pas Nécessaire en mode connecté

            DisplayProductsAndMenu();
        }

        public static void DeleteProduct()
        {

            Console.WriteLine("Veuillez saisir l'Id du produit à supprimer");
            int id = int.Parse(Console.ReadLine());

            Product product = _context.Product.Find(id);

            _context.Product.Remove(product);

            DisplayProductsAndMenu();
        }

        public static void SaveChanges()
        {
            try
            {
                _context.SaveChanges();
            }

            catch (DbUpdateConcurrencyException) // Intercepter l’erreur de concurrence d'accès
            {
                Console.WriteLine("Les données du produit ont déjà été modifiées, veuillez rafraichir vos données avant de saisir à nouveau votre requête");
                Console.ReadKey();
            }
        }

        public static void CancelChanges()
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
        }
    }
}
    


