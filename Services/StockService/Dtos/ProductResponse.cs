namespace StockService.Dtos
{
    public class ProductResponse
    {
        public required int  Id { get; init; }
        public required string Name { get; init; }

        public required string Description { get; init; }

        public required decimal Price { get; init; }

        public required int Quantity { get; init; }
    }
}