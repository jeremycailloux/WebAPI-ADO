using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Northwind2API_ADO.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Northwind2API_ADO.Data
{
    public class Northwind2Context //Ajouter à cette classe un constructeur qui lui fournira le singleton des paramètres.
    {
        private readonly string _connect;
        public Northwind2Context(IConfiguration config)
        {
            _connect = config.GetConnectionString("Northwind2Connect");
        }
        public List<String> GetCountries()
        {
            var listCountry = new List<String>();

            // On créé une commande et on définit le code sql à exécuter
            var cmd = new SqlCommand(); // On utilise la classe SqlCommand
            cmd.CommandText = @"select distinct A.Country from Supplier S inner join Address A on(S.AddressId  = A.AddressId)"; // Requête SQL

            // On crée une connexion à partir de la chaîne de connexion stockée
            // dans les paramètres de l'appli
            using (var cnx = new SqlConnection(_connect)) // On utilise la classe SqlConnection
            {
                // On affecte la connexion à la commande
                cmd.Connection = cnx;
                // On ouvre la connexion
                cnx.Open();

                // On exécute la commande, et on lit ses résultats avec un objet SqlDataRedader
                using (SqlDataReader sdr = cmd.ExecuteReader()) // On utilise la classe SqlDataReader
                {
                    // On lit les lignes de résultat une par une
                    while (sdr.Read())
                    {
                        //...et pour chacune on crée un objet qu'on ajoute à la liste
                        listCountry.Add((string)sdr["Country"]);
                    }
                }
            }
            // Le fait d'avoir créé la connexion dans une instruction using
            // permet de fermer cette connexion automatiquement à la fin du bloc using

            return listCountry;
        }

        // Récupérer la liste des fournisseurs d'un pays donné (2.3	Gestion de la valeur Null)
        public List<Supplier> GetSuppliers(string country)
        {
            var listSuppliers = new List<Supplier>();

            var cmd = new SqlCommand();
            cmd.CommandText = @"select S.SupplierId, S.CompanyName, S.HomePage from Supplier S
                                inner join Address A on(A.AddressId = S.AddressId)
                                where A.Country = @country";

            var param = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.NVarChar,
                ParameterName = "@country",
                Value = country
            };

            // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre
            cmd.Parameters.Add(param);

            using (var cnx = new SqlConnection(_connect))
            {
                cmd.Connection = cnx;
                cnx.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        var sup = new Supplier();
                        sup.CompanyName = (string)sdr["CompanyName"];
                        sup.SupplierId = (int)sdr["SupplierId"];

                        if (sdr["HomePage"] != DBNull.Value) // Test DBNull
                            sup.HomePage = (string)sdr["HomePage"];

                        listSuppliers.Add(sup);
                    }
                }
            }

            return listSuppliers;
        }

        // Récupération du Nombre de produits proposés par un fournisseur (2.4	Récupération d’une valeur unique)
        /*
        public int GetProductsCount(int supplierid)
        {

            var cmd = new SqlCommand();
            cmd.CommandText = @"select count(ProductId) from Product
                                where SupplierId = @supplierid";

            var param = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Int,
                ParameterName = "@supplierid",
                Value = supplierid
            };

            // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre
            cmd.Parameters.Add(param);

            using (var cnx = new SqlConnection(_connect))
            {
                cmd.Connection = cnx;
                cnx.Open();
                return (int)cmd.ExecuteScalar();
            }

        }
        */

        // Récupération du renvoie le nombre de produits fournis par l’ensemble des fournisseurs du pays choisi avec fonction scalaire
        public int GetProductsCount(string country)
        {

            var cmd = new SqlCommand();
            cmd.CommandText = @"select dbo.ufn_Country(@country)";

            var param = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.NVarChar,
                ParameterName = "@country",
                Value = country
            };

            // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre
            cmd.Parameters.Add(param);

            using (var cnx = new SqlConnection(_connect))
            {
                cmd.Connection = cnx;
                cnx.Open();
                return (int)cmd.ExecuteScalar();
            }

        }

        // Créer une méthode FindProducts permettant de récupérer les produits dont le nom contient une chaîne passée en paramètre
        // Les produits seront triés par ordre alphabétique de leur nom

        public List<Product> FindProducts(string search)
        {
            var listProducts = new List<Product>();

            var cmd = new SqlCommand();  // Mettre en œuvre une commande
            cmd.CommandText = @"select * 
                                from Product
                                where Name like ('%' + @search + '%')
                                order by Name";


            var param = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.NVarChar,
                ParameterName = "@search",
                Value = search
            };

            cmd.Parameters.Add(param); // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre

            using (var cnx = new SqlConnection(_connect)) // Mettre en œuvre une connexion
            {
                cmd.Connection = cnx;
                cnx.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader()) // Mettre en œuvre un DataReader
                {
                    while (sdr.Read())
                    {
                        var pro = new Product(); // On instancie un produit et on lui donne ses propriétés
                        pro.ProductId = (int)sdr["ProductId"];
                        pro.CategoryId = (Guid)sdr["CategoryId"];
                        pro.SupplierId = (int)sdr["SupplierId"];
                        pro.Name = (string)sdr["Name"];
                        pro.UnitPrice = (decimal)sdr["UnitPrice"];
                        pro.UnitsInStock = (short)sdr["UnitsInStock"];
                        // Pas de test DBnull car il n'y a rien de null
                        listProducts.Add(pro);
                    }
                }

                return listProducts;
            }

        }

        // Récupération d’un produit d’id donné

        public Product GetProduct(int productId)
        {

            var cmd = new SqlCommand();
            cmd.CommandText = @"select * from Product
                                where productId = @productId";

            var param = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Int,
                ParameterName = "@productId",
                Value = productId
            };

            // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre
            cmd.Parameters.Add(param);

            using (var cnx = new SqlConnection(_connect))
            {
                cmd.Connection = cnx;
                cnx.Open();
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    if (sdr.Read())
                    {
                        var pro = new Product();
                        pro.ProductId = (int)sdr["ProductId"];
                        pro.CategoryId = (Guid)sdr["CategoryId"];
                        pro.SupplierId = (int)sdr["SupplierId"];
                        pro.Name = (string)sdr["Name"];
                        pro.UnitPrice = (decimal)sdr["UnitPrice"];
                        pro.UnitsInStock = (short)sdr["UnitsInStock"];

                        return pro;
                    }

                    return null;
                }
            }

        }

        public int CreateProduct(Product newproduct)  // Creation d'un nouveau produit
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"insert Product(CategoryId, SupplierId, Name, UnitPrice, UnitsInStock)
                                values (@CategoryId, @SupplierId, @Name, @UnitPrice, @UnitsInStock)
                                SELECT cast(IDENT_CURRENT('Product') as int)"; // ProductId champ auto-incrémenté

            var param1 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                ParameterName = "@CategoryId",
                Value = newproduct.CategoryId
            };

            var param2 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Int,
                ParameterName = "@SupplierId",
                Value = newproduct.SupplierId
            };

            var param3 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.NVarChar,
                ParameterName = "@Name",
                Value = newproduct.Name
            };

            var param4 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Money,
                ParameterName = "@UnitPrice",
                Value = newproduct.UnitPrice
            };

            var param5 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.SmallInt,
                ParameterName = "@UnitsInStock",
                Value = newproduct.UnitsInStock
            };

            // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre
            cmd.Parameters.Add(param1);
            cmd.Parameters.Add(param2);
            cmd.Parameters.Add(param3);
            cmd.Parameters.Add(param4);
            cmd.Parameters.Add(param5);


            using (var cnx = new SqlConnection(_connect))
            {
                cmd.Connection = cnx;
                cnx.Open();
                int productid = (int)cmd.ExecuteScalar();
                return productid;
                // Pas besoin de SqlDataReader quand input
            }

        }

        public void UpdateProduct(Product product)  //Mise à jour d’un produit
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"update Product set CategoryId = @CategoryId, SupplierId = @SupplierId, Name = @Name, UnitPrice = @UnitPrice, UnitsInStock = @UnitsInStock
                                where ProductId = @ProductId";

            var param = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Int,
                ParameterName = "@ProductId",
                Value = product.ProductId
            };
            var param1 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.UniqueIdentifier,
                ParameterName = "@CategoryId",
                Value = product.CategoryId
            };

            var param2 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Int,
                ParameterName = "@SupplierId",
                Value = product.SupplierId
            };

            var param3 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.NVarChar,
                ParameterName = "@Name",
                Value = product.Name
            };

            var param4 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.Money,
                ParameterName = "@UnitPrice",
                Value = product.UnitPrice
            };

            var param5 = new SqlParameter
            {
                SqlDbType = System.Data.SqlDbType.SmallInt,
                ParameterName = "@UnitsInStock",
                Value = product.UnitsInStock
            };

            // Ajout à la collection des paramètres de la commande, obligatoire lorsqu'on a un paramètre
            cmd.Parameters.Add(param);
            cmd.Parameters.Add(param1);
            cmd.Parameters.Add(param2);
            cmd.Parameters.Add(param3);
            cmd.Parameters.Add(param4);
            cmd.Parameters.Add(param5);

            using (var cnx = new SqlConnection(_connect))
            {
                cmd.Connection = cnx;
                cnx.Open();
                cmd.ExecuteNonQuery();
            }
        }

        public int DeleteProduct(int productid)  // Suppression d’un produit et renvoi du nombre de lignes supprimées
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"delete from Product where ProductId = @productid";
            cmd.Parameters.Add(new SqlParameter
            {
                SqlDbType = SqlDbType.Int,
                ParameterName = "@productid",
                Value = productid
            });

            using (var cnx = new SqlConnection(_connect))
            {
                cnx.Open();
                cmd.Connection = cnx;
                return cmd.ExecuteNonQuery(); // Renvoi automatiquement le nombre de lignes affectées
            }
        }

        public void CreateProductCategory(Product product)
        {
            var cmd = new SqlCommand();
            cmd.CommandText = @"if not exists (select Name from Category where Name = '00000000-0000-0000-0000-000000000000')
                                insert Category (CategoryId, Name)
                                values (@CategoryId, 'Others')";
            cmd.Parameters.Add(new SqlParameter
            {
                SqlDbType = SqlDbType.UniqueIdentifier,
                ParameterName = "@CategoryId",
                Value = product.CategoryId
            });

        }


        //Mettre en œuvre une connexion, une commande et un DataReader pour récupérer des données d’une table de la base
    }