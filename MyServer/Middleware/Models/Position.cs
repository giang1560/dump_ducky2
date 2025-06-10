using System;
using System.Collections.Generic;

namespace Middleware.Models;

public partial class Position
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Level { get; set; }

    public virtual ICollection<LandPrice> LandPrices { get; set; } = new List<LandPrice>();
}
