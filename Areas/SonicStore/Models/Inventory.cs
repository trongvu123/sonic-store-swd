using System.ComponentModel.DataAnnotations.Schema;

namespace SonicStore.Areas.SonicStore.Models
{
    [Table("Storage")]
    public class Inventory
    {
        [Column("id")]
        public int Id { get; set; }
        [Column("storage")]
        public int Storage_capacity { get; set; }

        [Column("original_price")]
        public double OriginalPrice { get; set; }

        [Column("sale_price")]
        public double SalePrice { get; set; }
        [Column("quantity")]
        public int quantity { get; set; }

        [Column("product_id")]
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        [InverseProperty("Storages")]
        public virtual Product Product { get; set; } = null!;

        [InverseProperty("Storage")]
        public virtual ICollection<Cart> OrderDetails { get; set; } = new List<Cart>();

    }
}
