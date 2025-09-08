namespace StockService.Dtos
{
    public class StockResponse
    {
        public int ProductId { get; init; }
        public int QuantityAvailable { get; init; }
        public int QuantityReserved { get; init; }
    }
}