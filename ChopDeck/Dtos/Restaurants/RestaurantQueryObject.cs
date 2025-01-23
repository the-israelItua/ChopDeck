namespace ChopDeck.Dtos.Restaurants
{
    public class RestaurantQueryObject : PaginationQueryObject
    {
        public string? Name { get; set; } = string.Empty;
    }
}