using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class ShipmentInfo
{
    public int Id { get; set; }

    public int CityId { get; set; }

    public string StreetName { get; set; } = null!;

    public int BuildingNumber { get; set; }

    public int ApartmentNumber { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual Order? Order { get; set; }
}
