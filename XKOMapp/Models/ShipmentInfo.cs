using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class ShipmentInfo
{
    public int Id { get; set; }

    public string CityName { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public string BuildingNumber { get; set; } = null!;

    public string ApartmentNumber { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
