using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ShipmentInfo> ShipmentInfos { get; } = new List<ShipmentInfo>();
}
