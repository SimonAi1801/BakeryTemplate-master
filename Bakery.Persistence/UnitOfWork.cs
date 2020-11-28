using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Bakery.Core.Contracts;
using Bakery.Core.Entities;
using System.Collections.Generic;

namespace Bakery.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private bool _disposed;


        /// <summary>
        /// ConnectionString kommt aus den appsettings.json
        /// </summary>
        public UnitOfWork() : this(new ApplicationDbContext())
        {
        }

        public UnitOfWork(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            Customers = new CustomerRepository(_dbContext);
            Orders = new OrderRepository(_dbContext);
            OrderItems = new OrderItemRepository(_dbContext);
            Products = new ProductRepository(_dbContext);
        }


        public ICustomerRepository Customers { get; }
        public IOrderRepository Orders { get; }
        public IProductRepository Products { get; }
        public IOrderItemRepository OrderItems { get; }

        public async Task<int> SaveChangesAsync()
        {
            var entities = _dbContext.ChangeTracker.Entries()
                .Where(entity => entity.State == EntityState.Added
                                 || entity.State == EntityState.Modified)
                .Select(e => e.Entity);
            foreach (var entity in entities)
            {
                await ValidateEntity(entity);
            }
            return await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Validierungen auf DbContext-Ebene
        /// </summary>
        /// <param name="entity"></param>
        private async Task ValidateEntity(object entity)
        {
            ValidationContext validationContext;
            if (entity is Product product)
            {
                validationContext = new ValidationContext(product);
                if (await _dbContext.Products.AnyAsync(p => p.Id != product.Id
                    && p.Name.Equals(product.Name) && p.ProductNr.Equals(product.ProductNr)))
                {
                    throw new ValidationException(new ValidationResult(
                        $"Product mit dem Namen {product.Name} existiert bereits.",
                        new List<string> { "Product" }), null, new List<string> { "Product" });
                }
            }
            else if (entity is Order order)
            {
                validationContext = new ValidationContext(order);
                if (await _dbContext.Orders.AnyAsync(o => o.Id != order.Id
                    && o.OrderNr.Equals(order.OrderNr)))
                {
                    throw new ValidationException(new ValidationResult(
                        $"Bestellung mit der Nummer {order.OrderNr} existiert bereits.",
                        new List<string> { "Order" }), null, new List<string> { "Order" });
                }
            }
        }

        public async Task DeleteDatabaseAsync() => await _dbContext.Database.EnsureDeletedAsync();
        public async Task MigrateDatabaseAsync() => await _dbContext.Database.MigrateAsync();
        public async Task CreateDatabaseAsync() => await _dbContext.Database.EnsureCreatedAsync();

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true);
            GC.SuppressFinalize(this);
        }

        protected virtual async ValueTask DisposeAsync(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    await _dbContext.DisposeAsync();
                }
            }
            _disposed = true;
        }
    }
}
