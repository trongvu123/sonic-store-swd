using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SonicStore.Areas.SonicStore.Models;

[Table("Payment")]
public partial class Payment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("total_price")]
    public double? TotalPrice { get; set; }

    [Column("payment_method")]
    [StringLength(45)]
    public string? PaymentMethod { get; set; }

    [Column("transaction_date", TypeName = "datetime")]
    public DateTime? TransactionDate { get; set; }

    [InverseProperty("Payment")]
    public virtual Checkout? Order { get; set; }

    [InverseProperty("Payment")]
    public virtual ICollection<StatusPayment> StatusPayments { get; set; } = null!;
}
