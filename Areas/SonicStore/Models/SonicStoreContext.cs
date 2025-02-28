using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

public partial class SonicStoreContext : DbContext
{
    public SonicStoreContext()
    {
    }

    public SonicStoreContext(DbContextOptions<SonicStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<Category> Categories { get; set; }


    public virtual DbSet<Checkout> Orders { get; set; }

    public virtual DbSet<Cart> OrderDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Product> Products { get; set; }


    public virtual DbSet<ProductImage> ProductImages { get; set; }


    public virtual DbSet<Role> Roles { get; set; }


    public virtual DbSet<StatusOrder> StatusOrders { get; set; }
    public virtual DbSet<StatusPayment> StatusPayments { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserAddress> UserAddresses { get; set; }
    public virtual DbSet<Inventory> Storages { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=localhost,1433;Initial Catalog=SonicStore;User ID=sa;Password=123456;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3213E83F1227836C");

            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Inventory>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3213E83F07E0250E");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3213E83F4E6EE1EC");

        });



        modelBuilder.Entity<Checkout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3213E83FCBD4A6FA");

            entity.HasOne(o => o.OrderDetails)
    .WithMany(od => od.Orders)
    .HasForeignKey(o => o.CartId)
    .HasPrincipalKey(od => od.Id);

            entity.HasOne(d => d.Payment).WithOne(p => p.Order)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__payment_i__62E4AA3C");

        });

        modelBuilder.Entity<Cart>(entity =>
        {



            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasKey(od => new { od.StorageId, od.CustomerId, od.AddressId, od.Id });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3213E83F31074084");
        });
        modelBuilder.Entity<StatusPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__status_payment__3213E83F");
        });
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3213E83FD41103F3");

            entity.HasOne(d => d.BrandNavigation).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__brand_i__4924D839");
            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull);


        });

        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product___3213E83F88F164DB");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductImages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product_I__produ__4CF5691D");
        });



        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3213E83F5E85A4EC");
        });


        modelBuilder.Entity<StatusOrder>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Status__3213E83F97D90152");
        });
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3213E83FD23B5FEA");

            entity.HasOne(d => d.Account).WithOne(p => p.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User__account_id__3DB3258D");


        });

        modelBuilder.Entity<UserAddress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User_Add__3213E83F371BAF55");

            entity.HasOne(d => d.User).WithMany(p => p.UserAddresses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__User_Addr__user___4183B671");
        });


        OnModelCreatingPartial(modelBuilder);
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
