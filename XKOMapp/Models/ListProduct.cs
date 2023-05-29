using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class ListProduct
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ListId { get; set; }

    public int Number { get; set; }

    public virtual List List { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
