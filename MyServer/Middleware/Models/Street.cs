using System;
using System.Collections.Generic;

namespace Middleware.Models;

public partial class Street
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int DistrictId { get; set; }

    public virtual District District { get; set; } = null!;

    public virtual ICollection<LandPrice> LandPrices { get; set; } = new List<LandPrice>();
}
