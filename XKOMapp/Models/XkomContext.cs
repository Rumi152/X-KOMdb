﻿using System;
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

    public virtual DbSet<ReviewRating> ReviewRatings { get; set; }

    public virtual DbSet<ShipmentInfo> ShipmentInfos { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=XKOM;Trusted_Connection=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC271BAAA58B");

            entity.ToTable("Cart");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.PromoCodeId).HasColumnName("PromoCodeID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.PromoCode).WithMany(p => p.Carts)
                .HasForeignKey(d => d.PromoCodeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Cart__PromoCodeI__4D94879B");

            entity.HasOne(d => d.User).WithMany(p => p.Carts)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Cart__UserID__4CA06362");
        });

        modelBuilder.Entity<CartProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart_Pro__3214EC27061BF1A2");

            entity.ToTable("Cart_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.CartId)
                .HasConstraintName("FK__Cart_Prod__CartI__5441852A");

            entity.HasOne(d => d.Product).WithMany(p => p.CartProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Cart_Prod__Produ__534D60F1");
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__City__3214EC2721BB9A8E");

            entity.ToTable("City");

            entity.HasIndex(e => e.Name, "UQ__City__737584F6171799F0").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<FavouriteProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Favourit__3214EC270A2CD112");

            entity.ToTable("FavouriteProduct");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.FavouriteProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Favourite__Produ__52593CB8");

            entity.HasOne(d => d.User).WithMany(p => p.FavouriteProducts)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Favourite__UserI__5165187F");
        });

        modelBuilder.Entity<List>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__List__3214EC27403A7D8E");

            entity.ToTable("List");

            entity.HasIndex(e => e.Link, "UQ__List__B827DC69AC5A20B8").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Link)
                .HasMaxLength(128)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ListProduct>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__List_Pro__3214EC278633CB34");

            entity.ToTable("List_Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ListId).HasColumnName("ListID");
            entity.Property(e => e.ProductId).HasColumnName("ProductID");

            entity.HasOne(d => d.List).WithMany(p => p.ListProducts)
                .HasForeignKey(d => d.ListId)
                .HasConstraintName("FK__List_Prod__ListI__5629CD9C");

            entity.HasOne(d => d.Product).WithMany(p => p.ListProducts)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__List_Prod__Produ__5535A963");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Order__3214EC2717F15CD1");

            entity.ToTable("Order");

            entity.HasIndex(e => e.ShipmentInfoId, "UQ__Order__23189E8A35892998").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("CartID");
            entity.Property(e => e.NeedInstallationAssistance).HasDefaultValueSql("((0))");
            entity.Property(e => e.OrderDate).HasColumnType("datetime");
            entity.Property(e => e.PaymentMethodId).HasColumnName("PaymentMethodID");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.ShipmentInfoId).HasColumnName("ShipmentInfoID");
            entity.Property(e => e.StatusId).HasColumnName("StatusID");

            entity.HasOne(d => d.Cart).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__CartID__4E88ABD4");

            entity.HasOne(d => d.PaymentMethod).WithMany(p => p.Orders)
                .HasForeignKey(d => d.PaymentMethodId)
                .HasConstraintName("FK__Order__PaymentMe__5070F446");

            entity.HasOne(d => d.ShipmentInfo).WithOne(p => p.Order)
                .HasForeignKey<Order>(d => d.ShipmentInfoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__ShipmentI__571DF1D5");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Order__StatusID__4F7CD00D");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC275C24E00F");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PaymentMethod>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PaymentM__3214EC2734E193CA");

            entity.ToTable("PaymentMethod");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC271B1CB16B");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.Description)
                .HasMaxLength(512)
                .IsUnicode(false);
            entity.Property(e => e.IntroductionDate).HasColumnType("date");
            entity.Property(e => e.Name)
                .HasMaxLength(32)
                .IsUnicode(false);
            entity.Property(e => e.Picture).HasColumnType("image");
            entity.Property(e => e.Price).HasColumnType("money");
            entity.Property(e => e.Properties).IsUnicode(false);

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Product__Categor__47DBAE45");

            entity.HasOne(d => d.Company).WithMany(p => p.Products)
                .HasForeignKey(d => d.CompanyId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Product__Company__48CFD27E");
        });

        modelBuilder.Entity<ProductCategory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC27CB4DE683");

            entity.ToTable("ProductCategory");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ProductCompany>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ProductC__3214EC27A38FEBF6");

            entity.ToTable("ProductCompany");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(64)
                .IsUnicode(false);
        });

        modelBuilder.Entity<PromoCode>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__PromoCod__3214EC27826C4512");

            entity.ToTable("PromoCode");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Code)
                .HasMaxLength(16)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.MaximumMoney).HasColumnType("money");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Review__3214EC2713FCD2C3");

            entity.ToTable("Review");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Description).HasMaxLength(256);
            entity.Property(e => e.ProductId).HasColumnName("ProductID");
            entity.Property(e => e.RatingId).HasColumnName("RatingID");
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Product).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.ProductId)
                .HasConstraintName("FK__Review__ProductI__49C3F6B7");

            entity.HasOne(d => d.Rating).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.RatingId)
                .HasConstraintName("FK__Review__RatingID__4AB81AF0");

            entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Review__UserID__4BAC3F29");
        });

        modelBuilder.Entity<ReviewRating>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ReviewRa__3214EC27E3A6EFB6");

            entity.ToTable("ReviewRating");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Name)
                .HasMaxLength(16)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ShipmentInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Shipment__3214EC27631E62F1");

            entity.ToTable("ShipmentInfo");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CityId).HasColumnName("CityID");
            entity.Property(e => e.StreetName)
                .HasMaxLength(64)
                .IsUnicode(false);

            entity.HasOne(d => d.City).WithMany(p => p.ShipmentInfos)
                .HasForeignKey(d => d.CityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ShipmentI__CityI__5812160E");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__User__3214EC27A87E3F7A");

            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Email)
                .HasMaxLength(256)
                .IsUnicode(false);
            entity.Property(e => e.LastName).HasMaxLength(32);
            entity.Property(e => e.Name).HasMaxLength(32);
            entity.Property(e => e.Password)
                .HasMaxLength(32)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
