﻿using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class ProductCompany
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; } = new List<Product>();
}
