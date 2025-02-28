namespace SonicStore.Areas.SonicStore.Dtos
{
    public class OrderModel
    {
        public int? OrderId { get; set; }

        public DateTime? OrderDate { get; set; }

        public double? Total { get; set; }

        public string? Status { get; set; }

        public int Index { get; set; }

        public List<OrderDetailsModel> OrderDetails { get; set; } = new();
    }
}
