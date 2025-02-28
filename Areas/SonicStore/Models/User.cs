using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("User")]
[Index("AccountId", Name = "UQ__User__46A222CC47970D57", IsUnique = true)]
[Index("Email", Name = "UQ__User__AB6E6164D8A3A270", IsUnique = true)]
[Index("Phone", Name = "UQ__User__B43B145F81F86A8C", IsUnique = true)]
public partial class User
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("full_name")]
    [StringLength(50)]
    public string FullName { get; set; } = null!;

    [Column("dob", TypeName = "datetime")]
    public DateTime Dob { get; set; }

    [Column("email")]
    [StringLength(45)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Column("phone")]
    [StringLength(10)]
    [Phone]
    public string Phone { get; set; } = null!;

    [Column("gender")]
    [StringLength(45)]
    public string Gender { get; set; } = null!;

    [Column("update_date", TypeName = "datetime")]
    public DateTime UpdateDate { get; set; }

    [Column("update_by")]
    public int UpdateBy { get; set; }

    [Column("account_id")]
    public int AccountId { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }

    [ForeignKey("AccountId")]
    [InverseProperty("User")]
    public virtual Account? Account { get; set; } = null!;


    [InverseProperty("User")]
    public virtual ICollection<Cart> OrderDetails { get; set; } = new List<Cart>();



    [ForeignKey("RoleId")]
    [InverseProperty("Users")]

    public virtual Role? Role { get; set; } = null!;


    [InverseProperty("User")]
    public virtual ICollection<UserAddress> UserAddresses { get; set; } = new List<UserAddress>();
}
