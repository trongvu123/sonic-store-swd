using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Brand")]
public partial class Brand
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("brand_name")]
    [StringLength(100)]
    [Unicode(false)]
    public string BrandName { get; set; } = null!;
    [Column("brand_image")]
    [StringLength(100)]
    public string brandImage { get; set; } 

    [InverseProperty("BrandNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
