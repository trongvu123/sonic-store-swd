using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SonicStore.Areas.SonicStore.Models;

[Table("Checkout")]


public partial class Checkout
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("order_date", TypeName = "datetime")]
    public DateTime? OrderDate { get; set; }

    [Column("sale_id")]
    public int? SaleId { get; set; }

    [Column("cart_id")]
    public int? CartId { get; set; }

    [Column("payment_id")]
    public int PaymentId { get; set; }

    [Column("index")]
    public int index { get; set; }


    [ForeignKey("CartId")]
    [InverseProperty("Orders")]
    public virtual Cart OrderDetails { get; set; } = null!;

    [ForeignKey("PaymentId")]
    [InverseProperty("Order")]
    public virtual Payment Payment { get; set; } = null!;

    [InverseProperty("Order")]
    public virtual ICollection<StatusOrder> Status { get; set; } = null!;
}
