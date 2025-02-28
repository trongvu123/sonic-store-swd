using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SonicStore.Areas.SonicStore.Models
{
    [Table("status_payment")]
    public class StatusPayment
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
        [Column("payment_id")]
        public int Payment_id { get; set; }
        [ForeignKey("Payment_id")]
        [InverseProperty("StatusPayments")]
        public virtual Payment? Payment { get; set; }
    }
}
