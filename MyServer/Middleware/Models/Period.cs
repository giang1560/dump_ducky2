using System;
using System.Collections.Generic;

namespace Middleware.Models;

public partial class Period
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int StartYear { get; set; }

    public int EndYear { get; set; }

    public virtual ICollection<LandPrice> LandPrices { get; set; } = new List<LandPrice>();
}
