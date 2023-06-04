using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class ShipmentInfo
{
    public int Id { get; set; }

    public string CityName { get; set; } = null!;

    public string StreetName { get; set; } = null!;

    public int BuildingNumber { get; set; }

    public int ApartmentNumber { get; set; }

    public virtual Order? Order { get; set; }
}
