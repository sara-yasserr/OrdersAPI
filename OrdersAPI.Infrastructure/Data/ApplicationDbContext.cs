using Microsoft.EntityFrameworkCore;
using OrdersAPI.Core.Models;
using StackExchange.Redis;
using Order = OrdersAPI.Core.Models.Order;

namespace OrdersAPI.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Product).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Amount).HasPrecision(18, 2);
                entity.ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Order_Amount_MinValue", "Amount >= 1");
                });
            });
        }
    }
}