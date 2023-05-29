using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace XKOMapp.Models;

public partial class XkomContext : DbContext
{
    public XkomContext()
    {
    }

    public XkomContext(DbContextOptions<XkomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartProduct> CartProducts { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<FavouriteProduct> FavouriteProducts { get; set; }

    public virtual DbSet<List> Lists { get; set; }

    public virtual DbSet<ListProduct> ListProducts { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<ProductCompany> ProductCompanies { get; set; }

    public virtual DbSet<PromoCode> PromoCodes { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<ShipmentInfo> ShipmentInfos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=XKOM;Trusted_Connection=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC2759B8C9B3");

            entity.ToTable("Cart");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Discount).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Cart__UserID__5165187F");
        });

        modelBuilder.Entity<CartProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart_Pro__3214EC2702AACDD1");

            entity.ToTable("Cart_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__Cart_Prod__CartI__5812160E");

            entity.HasOne(d => d.Product).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart_Prod__Produ__571DF1D5");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__City__3214EC27B0211B15");

            entity.ToTable("City");

            entity.HasIndex(e => e.Name, "UQ__City__737584F6875CFD85").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FavouriteProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favourit__3214EC27E6B63C4E");

            entity.ToTable("FavouriteProduct");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.FavouriteProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Favourite__Produ__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.FavouriteProducts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Favourite__UserI__5535A963");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__List__3214EC27D9E26BC2");

            entity.ToTable("List");

            entity.HasIndex(e => e.Link, "idx_List_NullableUnique")
                .IsUnique()
                .HasFilter("([Link] IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Link)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasDefaultValueSql("('newlist')");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Lists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__List__UserID__5DCAEF64");
        });

        modelBuilder.Entity<ListProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__List_Pro__3214EC2774946BAA");

            entity.ToTable("List_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.List).WithMany(p => p.ListProducts)
                .HasForeignKey(d => d.ListId)
                .HasConstraintName("FK__List_Prod__ListI__59FA5E80");

            entity.HasOne(d => d.Product).WithMany(p => p.ListProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__List_Prod__Produ__59063A47");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC272E8E1FD9");

            entity.ToTable("Order");

            entity.HasIndex(e => e.ShipmentInfoId, "UQ__Order__23189E8AC796CE9A").IsUnique();

            entity.HasIndex(e => e.CartId, "UQ__Order__51BCD796B87604E2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ShipmentInfoId).HasColumnName("ShipmentInfoID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Cart).WithOne(p => p.Order)
                .HasForeignKey<Order>(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__CartID__52593CB8");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Order__PaymentMe__5441852A");

            entity.HasOne(d => d.ShipmentInfo).WithOne(p => p.Order)
                .HasForeignKey<Order>(d => d.ShipmentInfoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__ShipmentI__5AEE82B9");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__StatusID__534D60F1");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC2715C8793E");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.Name, "UQ__OrderSta__737584F6C5FD8D12").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC2731B91239");

            entity.ToTable("PaymentMethod");

            entity.HasIndex(e => e.Name, "UQ__PaymentM__737584F6993A5A5A").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC2766C6A615");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.IntroductionDate).HasColumnType("date");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Price).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.Properties).IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Product__Categor__4D94879B");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Product__Company__4E88ABD4");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC27A03937B8");

            entity.ToTable("ProductCategory");

            entity.HasIndex(e => e.Name, "UQ__ProductC__737584F6B98C504E").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProductCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC27D9D833EC");

            entity.ToTable("ProductCompany");

            entity.HasIndex(e => e.Name, "UQ__ProductC__737584F6376DB604").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PromoCod__3214EC279FF6EA53");

            entity.ToTable("PromoCode");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MaximumMoney).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC27FEEE4685");

            entity.ToTable("Review");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Review__ProductI__4F7CD00D");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Review__UserID__5070F446");
        });

        modelBuilder.Entity<ShipmentInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC27D62C6DAC");

            entity.ToTable("ShipmentInfo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.StreetName)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.City).WithMany(p => p.ShipmentInfos)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShipmentI__CityI__5BE2A6F2");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC274C07FC8E");

            entity.ToTable("User", tb => tb.HasTrigger("deleteActiveCart"));

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534FE7D2E17").IsUnique();

            entity.HasIndex(e => e.ActiveCartId, "idx_User_NullableUnique")
                .IsUnique()
                .HasFilter("([ActiveCartID] IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ActiveCartId).HasColumnName("ActiveCartID");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.LastName).HasMaxLength(32);
            entity.Property(e => e.Name).HasMaxLength(32);
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .IsUnicode(false);

            entity.HasOne(d => d.ActiveCart).WithOne(p => p.UserNavigation)
                .HasForeignKey<User>(d => d.ActiveCartId)
                .HasConstraintName("FK__User__ActiveCart__5CD6CB2B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
