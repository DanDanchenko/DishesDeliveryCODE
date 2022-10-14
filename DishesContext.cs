using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DishesDelivery.Models
{
    // Scaffold-DbContext "Server=(localdb)\MSSQLLocalDB;Database=Dishes;Trusted_Connection=True; MultipleActiveResultSets=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models -f -Context DishesContext
    


    public enum SiteViewMode { Login, Curier, Chef, User, Decline, Admin };
    public partial class DishesContext : DbContext
    {

        public static User ActiveUser;

        public static Order ActiveOrder;

        public static SiteViewMode ActiveSiteViewMode = SiteViewMode.Login;
        public DishesContext()
        {
            ActiveUser = new User();
            ActiveOrder = new Order();
            ActiveOrder.State = 0;

        }

        public void SetActiveOreder()
        {
            DishesContext.ActiveOrder = null;
            if (DishesContext.ActiveSiteViewMode == SiteViewMode.User)
            {
                var ordersListForActiveUser = Orders.Include(o => o.Client).Where(o => o.ClientId == DishesContext.ActiveUser.Id).Where(o => o.State > 0).Where(o => o.State < 10);
                if (ordersListForActiveUser.Count() > 0)
                {
                    DishesContext.ActiveOrder = ordersListForActiveUser.First();
                }
            }

            else
            {
                if (DishesContext.ActiveSiteViewMode == SiteViewMode.Curier)
                {
                    var ordersListForActiveUser = Orders.Include(o => o.Client).Where(o => o.CurierId == DishesContext.ActiveUser.Id).Where(o => o.State > 0).Where(o => o.State < 10);
                    if (ordersListForActiveUser.Count() > 0)
                    {
                        DishesContext.ActiveOrder = ordersListForActiveUser.First();
                    }
                }
            }
        }



        public DishesContext(DbContextOptions<DishesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Dish> Dishes { get; set; } = null!;
        public virtual DbSet<Ingradient> Ingradients { get; set; } = null!;
        public virtual DbSet<MigrationHistory> MigrationHistories { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderItem> OrderItems { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=Dishes;Trusted_Connection=True; MultipleActiveResultSets=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dish>(entity =>
            {
                entity.Property(e => e.Image).HasColumnType("image");
            });

            modelBuilder.Entity<Ingradient>(entity =>
            {
                entity.HasIndex(e => e.DishId, "IX_DishId");

                entity.HasIndex(e => e.ProductId, "IX_ProductId");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.Ingradients)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("FK_dbo.Ingradients_dbo.Dishes_DishId");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Ingradients)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_dbo.Ingradients_dbo.Products_ProductId");
            });

            modelBuilder.Entity<MigrationHistory>(entity =>
            {
                entity.HasKey(e => new { e.MigrationId, e.ContextKey })
                    .HasName("PK_dbo.__MigrationHistory");

                entity.ToTable("__MigrationHistory");

                entity.Property(e => e.MigrationId).HasMaxLength(150);

                entity.Property(e => e.ContextKey).HasMaxLength(300);

                entity.Property(e => e.ProductVersion).HasMaxLength(32);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasIndex(e => e.ClientId, "IX_ClientId");

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.CloseTime).HasColumnType("datetime");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("FK_dbo.Orders_dbo.Users_ClientId");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.HasIndex(e => e.DishId, "IX_DishId");

                entity.HasIndex(e => e.OrderId, "IX_OrderId");

                entity.HasOne(d => d.Dish)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.DishId)
                    .HasConstraintName("FK_dbo.OrderItems_dbo.Dishes_DishId");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_dbo.OrderItems_dbo.Orders_OrderId");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(20);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Name).HasMaxLength(30);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.RoleId, "IX_RoleId");

                entity.Property(e => e.Email).HasMaxLength(100);

                entity.Property(e => e.Image).HasColumnType("image");

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.Surname).HasMaxLength(30);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK_dbo.Users_dbo.Roles_RoleId");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
