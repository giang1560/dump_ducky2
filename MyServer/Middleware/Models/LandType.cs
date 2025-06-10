using System;
using System.Collections.Generic;

namespace Middleware.Models;

public partial class LandType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<LandPrice> LandPrices { get; set; } = new List<LandPrice>();
}
