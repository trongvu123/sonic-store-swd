using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("User_Address")]
public partial class UserAddress
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("user_address")]
    [StringLength(255)]
    public string User_Address { get; set; } = null!;
    [Column("status")]
    public bool Status { get; set; }

    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("UserAddresses")]

    public virtual User? User { get; set; } = null!;


    [InverseProperty("UserAddress")]
    public virtual ICollection<Cart> OrderDetails { get; set; } = new List<Cart>();
}
