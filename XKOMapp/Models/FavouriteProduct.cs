﻿using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class FavouriteProduct
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
