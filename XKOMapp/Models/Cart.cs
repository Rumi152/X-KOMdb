using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class Cart
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? PromoCodeId { get; set; }

    public virtual ICollection<CartProduct> CartProducts { get; } = new List<CartProduct>();

    public virtual ICollection<Order> Orders { get; } = new List<Order>();

    public virtual PromoCode? PromoCode { get; set; }

    public virtual User? User { get; set; }

    public virtual User? UserNavigation { get; set; }
}
