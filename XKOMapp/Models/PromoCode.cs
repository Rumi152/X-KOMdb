using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class PromoCode
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int Percentage { get; set; }

    public decimal? MaximumMoney { get; set; }

    public virtual ICollection<Cart> Carts { get; } = new List<Cart>();
}
