using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class List
{
    public int Id { get; set; }

    public string? Link { get; set; }

    public string Name { get; set; } = null!;

    public int UserId { get; set; }

    public virtual ICollection<ListProduct> ListProducts { get; } = new List<ListProduct>();

    public virtual User User { get; set; } = null!;
}
