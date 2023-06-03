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
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC274B99471E");

            entity.ToTable("Cart");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Discount).HasColumnType("decimal(8, 2)");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Cart__UserID__52593CB8");
        });

        modelBuilder.Entity<CartProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart_Pro__3214EC273F668705");

            entity.ToTable("Cart_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__Cart_Prod__CartI__59063A47");

            entity.HasOne(d => d.Product).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart_Prod__Produ__5812160E");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__City__3214EC2741D3E4D4");

            entity.ToTable("City");

            entity.HasIndex(e => e.Name, "UQ__City__737584F60C239A28").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FavouriteProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favourit__3214EC27CFE94A90");

            entity.ToTable("FavouriteProduct");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.FavouriteProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Favourite__Produ__571DF1D5");

            entity.HasOne(d => d.User).WithMany(p => p.FavouriteProducts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Favourite__UserI__5629CD9C");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__List__3214EC2705F09D2E");

            entity.ToTable("List");

            entity.HasIndex(e => e.Link, "idx_List_NullableUnique")
                .IsUnique()
                .HasFilter("([Link] IS NOT NULL)");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Link)
                .HasMaxLength(64)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false)
                .HasDefaultValueSql("('newlist')");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Lists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__List__UserID__5EBF139D");
        });

        modelBuilder.Entity<ListProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__List_Pro__3214EC27454E4843");

            entity.ToTable("List_Product");

            entity.HasIndex(e => new { e.ProductId, e.ListId }, "UC_ProductList").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.List).WithMany(p => p.ListProducts)
                .HasForeignKey(d => d.ListId)
                .HasConstraintName("FK__List_Prod__ListI__5AEE82B9");

            entity.HasOne(d => d.Product).WithMany(p => p.ListProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__List_Prod__Produ__59FA5E80");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC2779A04E87");

            entity.ToTable("Order");

            entity.HasIndex(e => e.ShipmentInfoId, "UQ__Order__23189E8A728C11BA").IsUnique();

            entity.HasIndex(e => e.CartId, "UQ__Order__51BCD796773C4052").IsUnique();

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
                .HasConstraintName("FK__Order__CartID__534D60F1");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Order__PaymentMe__5535A963");

            entity.HasOne(d => d.ShipmentInfo).WithOne(p => p.Order)
                .HasForeignKey<Order>(d => d.ShipmentInfoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__ShipmentI__5BE2A6F2");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__StatusID__5441852A");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC270957EF22");

            entity.ToTable("OrderStatus");

            entity.HasIndex(e => e.Name, "UQ__OrderSta__737584F645B649A1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC270A58665D");

            entity.ToTable("PaymentMethod");

            entity.HasIndex(e => e.Name, "UQ__PaymentM__737584F6572F173B").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC27E687CE59");

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
                .HasConstraintName("FK__Product__Categor__4E88ABD4");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Product__Company__4F7CD00D");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC2700597F5D");

            entity.ToTable("ProductCategory");

            entity.HasIndex(e => e.Name, "UQ__ProductC__737584F654C17DB2").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProductCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC276C962455");

            entity.ToTable("ProductCompany");

            entity.HasIndex(e => e.Name, "UQ__ProductC__737584F665ECA42D").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PromoCod__3214EC2714F3C9C6");

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
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC27ADBDB89D");

            entity.ToTable("Review");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Review__ProductI__5070F446");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Review__UserID__5165187F");
        });

        modelBuilder.Entity<ShipmentInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC27B83CA61C");

            entity.ToTable("ShipmentInfo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.StreetName)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.City).WithMany(p => p.ShipmentInfos)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShipmentI__CityI__5CD6CB2B");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC271BB14E87");

            entity.ToTable("User", tb => tb.HasTrigger("deleteActiveCart"));

            entity.HasIndex(e => e.Email, "UQ__User__A9D10534DE4C032C").IsUnique();

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
                .HasConstraintName("FK__User__ActiveCart__5DCAEF64");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
