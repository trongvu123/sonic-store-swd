namespace SonicStore.Areas.SonicStore.Dtos
{
    public class OrderDetailsModel
    {
        public int? OrderDetailsId { get; set; }

        public string? ProductName { get; set; } = null!;

        public string? ImageUrl { get; set; }

        public int? ProductId { get; set; }
        public int? CustomerId { get; set; }
        public int Quantity { get; set; }

        public double? Price { get; set; }

        public double? Total { get; set; }
    }
}
