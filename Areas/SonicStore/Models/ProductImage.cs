using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Product_Image")]
public partial class ProductImage
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("product_id")]
    public int ProductId { get; set; }

    [Column("image")]
    [StringLength(1000)]
    public string Image { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductImages")]
    public virtual Product Product { get; set; } = null!;
}
