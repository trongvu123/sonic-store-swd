using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Category")]
public partial class Category
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(45)]
    public string Name { get; set; } = null!;

	[InverseProperty("Category")]
	public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
