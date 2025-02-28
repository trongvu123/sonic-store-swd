using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[PrimaryKey("StorageId", "CustomerId", "AddressId", "Id")]
[Table("Cart")]
public partial class Cart
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("quantity")]
    public int? Quantity { get; set; }

    [Column("price")]
    public double? Price { get; set; }

    [Column("status")]
    public string? Status { get; set; }

    [Key]
    [Column("storage_id")]
    public int StorageId { get; set; }

    [Key]
    [Column("customer_id")]
    public int CustomerId { get; set; }

    [Key]
    [Column("user_address_id")]
    public int AddressId { get; set; }


    [InverseProperty("OrderDetails")]
    public virtual ICollection<Checkout> Orders { get; set; } = new List<Checkout>();

    [ForeignKey("StorageId")]
    [InverseProperty("OrderDetails")]
    public virtual Inventory Storage { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("OrderDetails")]
    public virtual User User { get; set; } = null!;

    [ForeignKey("AddressId")]
    [InverseProperty("OrderDetails")]
    public virtual UserAddress UserAddress { get; set; } = null!;
}
