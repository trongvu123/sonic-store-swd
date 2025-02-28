using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SonicStore.Areas.SonicStore.Models;

[Table("status_order")]
public partial class StatusOrder
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("type")]
    [StringLength(300)]
    public string? Type { get; set; }

    [Column("update_at", TypeName = "datetime")]
    public DateTime? UpdateAt { get; set; }

    [Column("update_by")]
    public int? UpdateBy { get; set; }

    [Column("create_at", TypeName = "datetime")]
    public DateTime? CreateAt { get; set; }

    [Column("create_by")]
    public int? CreateBy { get; set; }
    [Column("checkout_id")]
    public int OrderId { get; set; }

    [InverseProperty("Status")]
    [ForeignKey("OrderId")]
    public virtual Checkout? Order { get; set; }
}
