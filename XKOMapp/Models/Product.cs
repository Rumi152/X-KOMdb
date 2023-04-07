using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public string Description { get; set; } = null!;

    public byte[]? Picture { get; set; }

    public int? CategoryId { get; set; }

    public int? CompanyId { get; set; }

    public bool IsAvailable { get; set; }

    public DateTime? IntroductionDate { get; set; }

    public string? Properties { get; set; }

    public virtual ICollection<CartProduct> CartProducts { get; } = new List<CartProduct>();

    public virtual ProductCategory? Category { get; set; }

    public virtual ProductCompany? Company { get; set; }

    public virtual ICollection<FavouriteProduct> FavouriteProducts { get; } = new List<FavouriteProduct>();

    public virtual ICollection<ListProduct> ListProducts { get; } = new List<ListProduct>();

    public virtual ICollection<Review> Reviews { get; } = new List<Review>();
}
