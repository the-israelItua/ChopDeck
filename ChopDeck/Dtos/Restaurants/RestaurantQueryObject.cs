namespace ChopDeck.Helpers
{
    public class RestaurantQueryObject : PaginationQueryObject
    {
        public string? Name { get; set; } = string.Empty;
    }
}