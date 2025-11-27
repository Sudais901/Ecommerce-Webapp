using System;
using System.Collections.Generic;

namespace Ecommerce_Webapp.Models;

public partial class CartItem
{
    public int ProductId { get; set; }

    public string Pname { get; set; } = null!;

    public string? ImagePath { get; set; }

    public int Quantity { get; set; }

    public decimal Price { get; set; }
}
