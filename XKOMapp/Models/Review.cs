using System;
using System.Collections.Generic;

namespace XKOMapp.Models;

public partial class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Description { get; set; } = null!;

    public int RatingId { get; set; }

    public int? UserId { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ReviewRating Rating { get; set; } = null!;

    public virtual User? User { get; set; }
}
