using EComm.Core;
using EComm.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace EComm.Infrastructure
{
    public static class RepositoryFactory
    {
        public static IRepository Create(string connStr) =>
            new RepositoryEF(connStr);
    }

    internal class RepositoryEF : DbContext, IRepository
    {
        private readonly string _connStr;

        public RepositoryEF(string connStr)
        {
            _connStr = connStr;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connStr);
            optionsBuilder.LogTo(Console.WriteLine);
        }

        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();

        public async Task AddProduct(Product product)
        {
            Products.Add(product);
            await SaveChangesAsync();
        }

        public async Task<bool> DeleteProduct(Product product)
        {
            try {
                Products.Remove(product);
                int rowsAffected = await SaveChangesAsync();
                return (rowsAffected > 0);
            }
            catch (DbUpdateException ex) {
                throw new ApplicationException("Unable to delete the product because of an integrity constraint", ex);
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts(bool includeSuppliers = false)
        {
            return includeSuppliers switch {
                true => await Products.Include(p => p.Supplier).ToListAsync(),
                false => await Products.ToListAsync()
            };
        }

        public Task<IEnumerable<Supplier>> GetAllSuppliers()
        {
            throw new NotImplementedException();
        }

        public async Task<Product?> GetProduct(int id, bool includeSupplier = false)
        {
            return includeSupplier switch {
                true => await Products.Include(p => p.Supplier).SingleOrDefaultAsync(p => p.Id == id),
                false => await Products.SingleOrDefaultAsync(p => p.Id == id)
            };
        }

        public async Task<bool> SaveProduct(Product product)
        {
            Products.Attach(product);
            Entry(product).State = EntityState.Modified;
            int rowsAffected = await SaveChangesAsync();
            return (rowsAffected > 0);
        }
    }
}
