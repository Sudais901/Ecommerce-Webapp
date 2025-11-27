using System;
using System.Collections.Generic;

namespace Ecommerce_Webapp.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;
}
