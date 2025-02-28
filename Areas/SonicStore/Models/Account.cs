using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Account")]
[Index("Username", Name = "UQ__Account__F3DBC572D661B57A", IsUnique = true)]
public partial class Account
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("username")]
    [StringLength(45)]
    public string Username { get; set; } = null!;

    [Column("password")]
    [StringLength(100)]
    public string Password { get; set; }

    [Column("register_date")]
    public DateOnly RegisterDate { get; set; }

    [Column("status")]
    [StringLength(45)]

    public string Status { get; set; } = "on";

    [Column("google_account")]
    public bool GoogleAccountStatus { get; set; }



    [Column("by_admin")]
    public bool ByAdmin { get; set; }

    [InverseProperty("Account")]
    public virtual User? User { get; set; }
}