using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Role")]
public partial class Role
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("role_name")]
    [StringLength(30)]
    public string RoleName { get; set; } = null!;
	[InverseProperty("Role")]
	public virtual ICollection<User> Users { get; set; }    
}
