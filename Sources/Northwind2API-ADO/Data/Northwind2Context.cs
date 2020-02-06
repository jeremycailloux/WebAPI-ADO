using Microsoft.Extensions.Configuration;
using Northwind2API_ADO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Northwind2API_ADO.Data
{
   public class Northwind2Context
   {
      private readonly string _connect;

      public Northwind2Context(IConfiguration config)
      {
         _connect = config.GetConnectionString("Northwind2Connect");
      }

      // Requête select avec DataReader
      // Récupère la liste des pays des fournisseurs
      public IList<string> GetCountries()
      {
         var list = new List<string>();
         var cmd = new SqlCommand();
         cmd.CommandText = @"select distinct A.Country
				from Address A
				inner join Supplier S on S.AddressId = A.AddressId
				order by 1";

         using (var conn = new SqlConnection(_connect))
         {
            cmd.Connection = conn;
            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  list.Add((string)reader["Country"]);
               }
            }
         }

         return list;
      }

      // Requête select avec DataReader et paramètre
      // Récupère la liste de tous les fournisseurs d'un pays donné
      public List<Supplier> GetSuppliers(string pays)
      {
         var list = new List<Supplier>();
         var cmd = new SqlCommand();
         cmd.CommandText = @"select S.SupplierId, S.CompanyName, S.HomePage
							from Supplier S
							inner join Address A on S.AddressId = A.AddressId
							where A.Country = @pays
							order by 1";

         cmd.Parameters.Add(new SqlParameter
         {
            SqlDbType = SqlDbType.NVarChar,
            ParameterName = "@pays",
            Value = pays
         });

         using (var conn = new SqlConnection(_connect))
         {
            cmd.Connection = conn;
            conn.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  var item = new Supplier();
                  item.SupplierId = (int)reader["SupplierId"];
                  item.CompanyName = (string)reader["CompanyName"];
                  if (reader["HomePage"] != DBNull.Value)
                     item.HomePage = (string)reader["HomePage"];

                  list.Add(item);
               }
            }
         }

         return list;
      }

      // Requête select scalaire avec paramètre
      // Récupère le nombre de produits fournis par un fournisseur
      public int GetProductsCount(int supplierId)
      {
         var cmd = new SqlCommand();
         cmd.CommandText = @"select count(*) ProductsCount 
               from product where SupplierId = @id";

         cmd.Parameters.Add(new SqlParameter
         {
            SqlDbType = SqlDbType.Int,
            ParameterName = "@id",
            Value = supplierId
         });

         using (var cnx = new SqlConnection(_connect))
         {
            cmd.Connection = cnx;
            cnx.Open();
            return (int)cmd.ExecuteScalar();
         }
      }

      // Liste des produits dont le nom contient un texte donné
      public List<Product> FindProducts(string txt)
      {
         var products = new List<Product>();

         var cmd = new SqlCommand();
         cmd.CommandText = @"select ProductId, CategoryId, Name, SupplierId, UnitPrice, UnitsInStock
							from Product where Name like @txt order by Name";

         cmd.Parameters.Add(new SqlParameter
         {
            SqlDbType = SqlDbType.NVarChar,
            ParameterName = "@txt",
            Value = "%" + txt + "%" // les % ne peuvent pas être mis dans la requête
         });

         using (var cnx = new SqlConnection(_connect))
         {
            cmd.Connection = cnx;
            cnx.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               while (reader.Read())
               {
                  var prod = new Product();
                  prod.ProductId = (int)reader["ProductId"];
                  prod.CategoryId = (Guid)reader["CategoryId"];
                  prod.Name = (string)reader["Name"];
                  prod.SupplierId = (int)reader["SupplierId"];
                  prod.UnitPrice = (decimal)reader["UnitPrice"];
                  prod.UnitsInStock = (short)reader["UnitsInStock"];
                  products.Add(prod);
               }
            }
         }

         return products;
      }

      // Renvoie le produit dont l'id est passé en paramètre
      public Product GetProduct(int id)
      {
         Product prod = null;

         var cmd = new SqlCommand();
         cmd.CommandText = @"select ProductId, Name, CategoryId, SupplierId, UnitPrice, UnitsInStock
							from Product where ProductId = @id order by 1";

         cmd.Parameters.Add(new SqlParameter
         {
            SqlDbType = SqlDbType.Int,
            ParameterName = "@id",
            Value = id
         });

         using (var cnx = new SqlConnection(_connect))
         {
            cmd.Connection = cnx;
            cnx.Open();

            using (SqlDataReader reader = cmd.ExecuteReader())
            {
               if (reader.Read())
               {
                  prod = new Product();
                  prod.ProductId = (int)reader["ProductId"];
                  prod.CategoryId = (Guid)reader["CategoryId"];
                  prod.Name = (string)reader["Name"];
                  prod.SupplierId = (int)reader["SupplierId"];
                  prod.UnitPrice = (decimal)reader["UnitPrice"];
                  prod.UnitsInStock = (short)reader["UnitsInStock"];
               }
            }
         }

         return prod;
      }

      // Insère le produit passé en paramètre dans la base et renvoie son Id
      public int CreateProduct(Product produit)
      {
         int id = 0;

         var cmd = new SqlCommand();
         cmd.CommandText = @"insert Product (Name, CategoryId, SupplierId, UnitPrice, UnitsInStock)
									values (@Nom, @Cate, @Fourni, @PU, @Stock);
                     select CAST(IDENT_CURRENT('Product') as INT)";

         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@Nom", Value = produit.Name });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@Cate", Value = produit.CategoryId });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Fourni", Value = produit.SupplierId });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Decimal, ParameterName = "@PU", Value = produit.UnitPrice });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Stock", Value = produit.UnitsInStock });

         using (var cnx = new SqlConnection(_connect))
         {
            cmd.Connection = cnx;
            cnx.Open();
            id = (int)cmd.ExecuteScalar();
         }

         return id;
      }

      // Mete à jour le produit passé en paramètre dans la base
      public void UpdateProduct(Product produit)
      {
         var cmd = new SqlCommand();
         cmd.CommandText = @"update Product set Name = @Nom, CategoryId = @Cate,
							SupplierId = @Fourni, UnitPrice = @PU, UnitsInStock = @Stock
							where ProductId = @Id";
         
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Id", Value = produit.ProductId });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@Nom", Value = produit.Name });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@Cate", Value = produit.CategoryId });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Fourni", Value = produit.SupplierId });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Decimal, ParameterName = "@PU", Value = produit.UnitPrice });
         cmd.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Stock", Value = produit.UnitsInStock });

         using (var cnx = new SqlConnection(_connect))
         {
            cmd.Connection = cnx;
            cnx.Open();
            cmd.ExecuteNonQuery();
         }
      }


      // Requête delete - suppression d'un produit
      // Si le produit est référencé par une commande, la requête lève une
      // SqlException avec le N°547, qu'on intercepte dans le code appelant
      public int DeleteProduct(int id)
      {
         int res = 0;
         var cmd = new SqlCommand();
         cmd.CommandText = @"delete from Product where ProductId = @id";
         cmd.Parameters.Add(new SqlParameter
         {
            SqlDbType = SqlDbType.Int,
            ParameterName = "@id",
            Value = id
         });

         using (var conn = new SqlConnection(_connect))
         {
            cmd.Connection = conn;
            conn.Open();
            res = cmd.ExecuteNonQuery();
         }

         return res;
      }

      // Requêtes dans une transaction - création d'un produit et de la catégories Others
      public void CreateProductCategory(Product produit)
      {
         // Commande pour la création de la catégorie "Others" si nécessaire
         SqlCommand cmd1 = null;
         if (produit.CategoryId == Guid.Empty)
         {
            cmd1 = new SqlCommand();
            cmd1.CommandText = @"if not exists(
					select CategoryId from Category where CategoryId=@Cate)
					insert Category(CategoryId, Name, Description) values
					(@Cate, 'Others', 'Other products')";

            Guid idCategorieAutres = new Guid("D77CBDA2-DD29-4CD6-B86D-B033A9ADC632");
            cmd1.Parameters.Add(new SqlParameter
            {
               SqlDbType = SqlDbType.UniqueIdentifier,
               ParameterName = "@Cate",
               Value = idCategorieAutres
            });

            produit.CategoryId = idCategorieAutres;
         }

         // Commande pour la création du produit
         var cmd2 = new SqlCommand();
         cmd2.CommandText = @"insert Product (Name, CategoryId, SupplierId, UnitPrice, UnitsInStock)
									values (@Nom, @Cate, @Fourni, @PU, @Stock)";
         cmd2.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.NVarChar, ParameterName = "@Nom", Value = produit.Name });
         cmd2.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@Cate", Value = produit.CategoryId });
         cmd2.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Fourni", Value = produit.SupplierId });
         cmd2.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Decimal, ParameterName = "@PU", Value = produit.UnitPrice });
         cmd2.Parameters.Add(new SqlParameter { SqlDbType = SqlDbType.Int, ParameterName = "@Stock", Value = produit.UnitsInStock });

         using (var conn = new SqlConnection(_connect))
         {
            conn.Open();

            // Exécution des deux commandes au sein d'une transaction
            using (SqlTransaction tran = conn.BeginTransaction())
            {
               if (cmd1 != null)
               {
                  cmd1.Connection = conn;
                  cmd1.Transaction = tran;
                  cmd1.ExecuteNonQuery();
               }
               cmd2.Connection = conn;
               cmd2.Transaction = tran;
               cmd2.ExecuteNonQuery();

               tran.Commit();
            }
         }
      }
   }
}
