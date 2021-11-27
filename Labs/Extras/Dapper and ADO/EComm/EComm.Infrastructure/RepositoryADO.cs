using EComm.Core;
using EComm.Core.Entities;
using Microsoft.Data.SqlClient;

namespace EComm.Infrastructure
{
    public static class RepositoryAdoFactory
    {
        public static IRepository Create(string connStr) =>
            new RepositoryADO(connStr);
    }

    internal class RepositoryADO : IRepository
    {
        private readonly string _connStr;

        public RepositoryADO(string connStr)
        {
            _connStr = connStr;
        }

        public Task AddProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProduct(Product product)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> GetAllProducts(bool includeSuppliers = false)
        {
            var retVal = new List<Product>();
            string sql = includeSuppliers switch {
                false => "SELECT * FROM Products",
                true => "SELECT [p].[Id], [p].[IsDiscontinued], [p].[Package], [p].[ProductName], " +
                        "[p].[SupplierId], [p].[UnitPrice], [s].[Id] AS SId, [s].[City], " +
                        "[s].[CompanyName], [s].[ContactName], [s].[Country], [s].[Fax], [s].[Phone] " +
                        "FROM[Products] AS [p] INNER JOIN [Suppliers] AS[s] ON [p].[SupplierId] = [s].[Id]"
            };
            using SqlConnection conn = new SqlConnection(_connStr);
            using SqlCommand cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using SqlDataReader rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                var product = new Product {
                    Id = (int)rdr["Id"],
                    ProductName = (string)rdr["ProductName"],
                    UnitPrice = (decimal)rdr["UnitPrice"],
                    Package = (string)rdr["Package"],
                    IsDiscontinued = (bool)rdr["IsDiscontinued"],
                    SupplierId = (int)rdr["SupplierId"]
                };
                if (includeSuppliers) {
                    product.Supplier = new Supplier {
                        Id = (int)rdr["SId"],
                        City = (string)rdr["City"],
                        CompanyName = (string)rdr["CompanyName"],
                        ContactName = (string)rdr["ContactName"],
                        Country = (string)rdr["Country"],
                        Fax = (string)rdr["Country"],
                        Phone = (string)rdr["Phone"]
                    };
                }
                retVal.Add(product);
            }
            return retVal;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            var retVal = new List<Supplier>();
            string sql = "SELECT * FROM Suppliers";

            using SqlConnection conn = new SqlConnection(_connStr);
            using SqlCommand cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using SqlDataReader rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                retVal.Add(new Supplier {
                    Id = (int)rdr["Id"],
                    City = (string)rdr["City"],
                    CompanyName = (string)rdr["CompanyName"],
                    ContactName = (string)rdr["ContactName"],
                    Country = (string)rdr["Country"],
                    Fax = (string)rdr["Country"],
                    Phone = (string)rdr["Phone"]
                });
            }
            return retVal;
        }

        public async Task<Product?> GetProduct(int id, bool includeSupplier = false)
        {
            var retVal = new List<Product>();
            string sql = includeSupplier switch {
                false => "SELECT * FROM Products WHERE Id=@Id",
                true => "SELECT [p].[Id], [p].[IsDiscontinued], [p].[Package], [p].[ProductName], " +
                        "[p].[SupplierId], [p].[UnitPrice], [s].[Id] AS SId, [s].[City], " +
                        "[s].[CompanyName], [s].[ContactName], [s].[Country], [s].[Fax], [s].[Phone] " +
                        "FROM[Products] AS [p] INNER JOIN [Suppliers] AS[s] ON [p].[SupplierId] = [s].[Id] " +
                        "WHERE [p].Id = @Id"
            };
            using SqlConnection conn = new SqlConnection(_connStr);
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("Id", id);
            await conn.OpenAsync();
            using SqlDataReader rdr = await cmd.ExecuteReaderAsync();

            while (await rdr.ReadAsync()) {
                var product = new Product {
                    Id = (int)rdr["Id"],
                    ProductName = (string)rdr["ProductName"],
                    UnitPrice = (decimal)rdr["UnitPrice"],
                    Package = (string)rdr["Package"],
                    IsDiscontinued = (bool)rdr["IsDiscontinued"],
                    SupplierId = (int)rdr["SupplierId"]
                };
                if (includeSupplier) {
                    product.Supplier = new Supplier {
                        Id = (int)rdr["SId"],
                        City = (string)rdr["City"],
                        CompanyName = (string)rdr["CompanyName"],
                        ContactName = (string)rdr["ContactName"],
                        Country = (string)rdr["Country"],
                        Fax = (string)rdr["Country"],
                        Phone = (string)rdr["Phone"]
                    };
                }
                retVal.Add(product);
            }
            return (retVal.Count > 0 ? retVal[0] : null);
        }

        public async Task<bool> SaveProduct(Product product)
        {
            string sql = "UPDATE Products SET " +
                         "ProductName=@ProductName, " +
                         "UnitPrice=@UnitPrice, " +
                         "Package=@Package, " +
                         "IsDiscontinued=@IsDiscontinued, " +
                         "SupplierId=@SupplierId " +
                         "WHERE Id=@Id";

            using SqlConnection conn = new SqlConnection(_connStr);
            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("Id", product.Id);
            cmd.Parameters.AddWithValue("ProductName", product.ProductName);
            cmd.Parameters.AddWithValue("UnitPrice", product.UnitPrice);
            cmd.Parameters.AddWithValue("Package", product.Package);
            cmd.Parameters.AddWithValue("IsDiscontinued", product.IsDiscontinued);
            cmd.Parameters.AddWithValue("SupplierId", product.SupplierId);
            await conn.OpenAsync();
            int n = await cmd.ExecuteNonQueryAsync();
            return (n == 1);
        }
    }
}
