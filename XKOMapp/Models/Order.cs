using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class Order
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int StatusId { get; set; }

    public DateTime OrderDate { get; set; }

    public int? PaymentMethodId { get; set; }

    public decimal Price { get; set; }

    public int ShipmentInfoId { get; set; }

    public bool NeedInstallationAssistance { get; set; }

    public decimal? Discount { get; set; }

    public virtual Cart Cart { get; set; } = null!;

    public virtual PaymentMethod? PaymentMethod { get; set; }

    public virtual ShipmentInfo ShipmentInfo { get; set; } = null!;

    public virtual OrderStatus Status { get; set; } = null!;
}
