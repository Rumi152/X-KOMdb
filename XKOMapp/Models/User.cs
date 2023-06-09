﻿using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Email { get; set; } = null!;

    public int? ActiveCartId { get; set; }

    public virtual Cart? ActiveCart { get; set; }

    public virtual ICollection<Cart> Carts { get; } = new List<Cart>();

    public virtual ICollection<FavouriteProduct> FavouriteProducts { get; } = new List<FavouriteProduct>();

    public virtual ICollection<List> Lists { get; } = new List<List>();

    public virtual ICollection<Review> Reviews { get; } = new List<Review>();
}
