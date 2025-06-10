using System;
using System.Collections.Generic;

namespace Middleware.Models;

public partial class LandPrice
{
    public int Id { get; set; }

    public int PeriodId { get; set; }

    public int StreetId { get; set; }

    public int LandTypeId { get; set; }

    public int PositionId { get; set; }

    public decimal Price { get; set; }

    public virtual LandType LandType { get; set; } = null!;

    public virtual Period Period { get; set; } = null!;

    public virtual Position Position { get; set; } = null!;

    public virtual Street Street { get; set; } = null!;
}
