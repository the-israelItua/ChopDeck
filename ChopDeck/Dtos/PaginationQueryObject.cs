namespace ChopDeck.Dtos
{
    public class PaginationQueryObject
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}