using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MMStoreServer.Models;

namespace MMStoreServer.Repositories {

  public class MMStoreDBContext : DbContext {

    public MMStoreDBContext (DbContextOptions<MMStoreDBContext> options): base(options) {}
    public DbSet<MMStoreServer.Models.MMSUser> MMSUsers { get; set; }
    public DbSet<MMStoreServer.Models.MMSCategory> MMSCategories { get; set; }
    public DbSet<MMStoreServer.Models.MMSProduct> MMSProducts { get; set; }
    public DbSet<MMStoreServer.Models.MMSCartItem> MMSCarts { get; set; }
    public DbSet<MMStoreServer.Models.MMSOrder> MMSOrders { get; set; }
    public DbSet<MMStoreServer.Models.MMSOrderDetail> MMSOrderDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<MMSCartItem>().HasKey(c => new { c.UserID, c.ProductID });
    modelBuilder.Entity<MMSOrderDetail>().HasKey(d => new { d.OrderID, d.ProductID });
    modelBuilder.Entity<MMSOrderDetail>().Property(p => p.OrderPrice).HasColumnType("money");
    modelBuilder.Entity<MMSProduct>().Property(p => p.ProductPrice).HasColumnType("money");
    }

  }
}
