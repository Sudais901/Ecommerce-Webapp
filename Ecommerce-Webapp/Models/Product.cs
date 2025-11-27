using System;
using System.Collections.Generic;

namespace Ecommerce_Webapp.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string? Pname { get; set; }

    public decimal? Price { get; set; }

    public string? Description { get; set; }

    public string? ImagePath { get; set; }

    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
}
