using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Product")]
public partial class Product
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(100)]
    public string Name { get; set; } = null!;

    [Column("detail")]
    public string Detail { get; set; } = null!;

    [Column("image")]
    [StringLength(100)]
    public string Image { get; set; } = null!;

    [Column("update_date", TypeName = "datetime")]
    public DateTime? UpdateDate { get; set; }

    [Column("status")]
    public bool Status { get; set; }

    [Column("category_id")]
    public int CategoryId { get; set; }

    [Column("brand_id")]
    public int BrandId { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand BrandNavigation { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

    [InverseProperty("Product")]
    public virtual ICollection<Inventory> Storages { get; set; } = new List<Inventory>();

}
