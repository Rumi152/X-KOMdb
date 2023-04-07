using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class ReviewRating
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; } = new List<Review>();
}
