using System;
using System.Collections.Generic;

namespace Middleware.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public DateTime LastLogin { get; set; }
}

public class CreateUserDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
}
