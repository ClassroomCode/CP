using EComm.Core;
using EComm.Core.Entities;
using Microsoft.Data.SqlClient;
using Dapper;

namespace EComm.Infrastructure
{
    public static class RepositoryDapperFactory
    {
        public static IRepository Create(string connStr) =>
            new RepositoryDapper(connStr);
    }

    internal class RepositoryDapper : IRepository
    {
        private readonly string _connStr;

        public RepositoryDapper(string connStr)
        {
            _connStr = connStr;
        }

        public async Task AddProduct(Product product)
        {
            string sql = "INSERT INTO Products (ProductName, SupplierId, UnitPrice, Package, IsDiscontinued) " +
                "VALUES (@ProductName, @SupplierId, @UnitPrice, @Package, @IsDiscontinued);";

            using var conn = new SqlConnection(_connStr);

            await conn.ExecuteAsync(sql, new { 
                ProductName = product.ProductName,
                SupplierId = product.SupplierId,
                UnitPrice = product.UnitPrice,
                Package = product.Package,
                IsDiscontinued = product.IsDiscontinued
            });
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            string sql = "DELETE FROM Products WHERE Id = @Id;";

            using var conn = new SqlConnection(_connStr);

            try {
                int rowsAffected = await conn.ExecuteAsync(sql, new { Id = product.Id, });
                return (rowsAffected > 0);
            }
            catch (Exception ex) {
                throw new ApplicationException("Unable to delete the product because of an integrity constraint", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts(bool includeSuppliers = false)
        {
            string sql = includeSuppliers switch {
                false => "SELECT * FROM Products",
                true => "SELECT [p].[Id], [p].[IsDiscontinued], [p].[Package], [p].[ProductName], " +
                        "[p].[SupplierId], [p].[UnitPrice], [s].[Id] AS SId, [s].[City], " +
                        "[s].[CompanyName], [s].[ContactName], [s].[Country], [s].[Fax], [s].[Phone] " +
                        "FROM[Products] AS [p] INNER JOIN [Suppliers] AS[s] ON [p].[SupplierId] = [s].[Id]"
            };
            using var conn = new SqlConnection(_connStr);

            if (!includeSuppliers) {
                var products = await conn.QueryAsync<Product>(sql);
                return products.ToList();
            }
            var results = await conn.QueryAsync(sql);
            // mapping could can be made simpler by using a 3rd-party mapping library
            var retVal = new List<Product>();
            foreach (var result in results) {
                retVal.Add(new Product {
                    Id = result.Id,
                    ProductName = result.ProductName,
                    UnitPrice = result.UnitPrice,
                    Package = result.Package,
                    IsDiscontinued = result.IsDiscontinued,
                    SupplierId = result.SupplierId,
                    Supplier = new Supplier {
                        Id = result.SId,
                        City = result.City,
                        CompanyName = result.CompanyName,
                        ContactName = result.ContactName,
                        Country = result.Country,
                        Fax = result.Country,
                        Phone = result.Phone
                    }
                });
            }
            return retVal;
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            string sql = "SELECT * FROM Suppliers";
            using var conn = new SqlConnection(_connStr);
            var suppliers = await conn.QueryAsync<Supplier>(sql);
            return suppliers.ToList();
        }

        public async Task<Product?> GetProduct(int id, bool includeSupplier = false)
        {
            string sql = includeSupplier switch {
                false => "SELECT * FROM Products WHERE [p].Id=@Id",
                true => "SELECT [p].[Id], [p].[IsDiscontinued], [p].[Package], [p].[ProductName], " +
                        "[p].[SupplierId], [p].[UnitPrice], [s].[Id] AS SId, [s].[City], " +
                        "[s].[CompanyName], [s].[ContactName], [s].[Country], [s].[Fax], [s].[Phone] " +
                        "FROM [Products] AS [p] INNER JOIN [Suppliers] AS[s] ON [p].[SupplierId] = [s].[Id] " +
                        "WHERE [p].Id=@Id"
            };
            using var conn = new SqlConnection(_connStr);

            if (!includeSupplier) {
                var products = await conn.QueryAsync<Product>(sql, new { Id = id });
                return products.SingleOrDefault();
            }
            var results = await conn.QueryAsync(sql, new { Id = id });
            // mapping could be made simpler by using a 3rd-party mapping library
            var retVal = new List<Product>();
            foreach (var result in results) {
                retVal.Add(new Product {
                    Id = result.Id,
                    ProductName = result.ProductName,
                    UnitPrice = result.UnitPrice,
                    Package = result.Package,
                    IsDiscontinued = result.IsDiscontinued,
                    SupplierId = result.SupplierId,
                    Supplier = new Supplier {
                        Id = result.SId,
                        City = result.City,
                        CompanyName = result.CompanyName,
                        ContactName = result.ContactName,
                        Country = result.Country,
                        Fax = result.Country,
                        Phone = result.Phone
                    }
                });
            }
            return retVal.SingleOrDefault();
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

            using var conn = new SqlConnection(_connStr);
            int n = await conn.ExecuteAsync(sql, new {
                Id = product.Id,
                ProductName = product.ProductName,
                UnitPrice = product.UnitPrice,
                Package = product.Package,
                IsDiscontinued = product.IsDiscontinued,
                SupplierId = product.SupplierId
            });
            return (n == 1);
        }
    }
}
