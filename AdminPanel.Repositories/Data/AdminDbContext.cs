using AdminPanel.Core.Entities;
using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.Entities.Categories;
using AdminPanel.Core.Entities.Identity;
using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.PurchaseInvoices;
using AdminPanel.Core.Entities.Reports;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AdminPanel.Repositories.Data
{
    public class AdminDbContext : IdentityDbContext<ApplicationUser>
    {
        public AdminDbContext(DbContextOptions<AdminDbContext> options)
            :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(builder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken ct = default)
        {
            var now = DateTime.UtcNow;
            foreach(var entry in ChangeTracker.Entries<ModelBase>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.LastModifiedAt = now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedAt = now;
                        // For not change In Create At After Modified
                        entry.Property(X => X.CreatedAt).IsModified = false;
                        break;
                }
            }
            return base.SaveChangesAsync(ct);
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ReportTransaction> ReportTransactions { get; set; }
        public DbSet<PurchaseInvoice> PurchaseInvoices { get; set; }
    }
}
