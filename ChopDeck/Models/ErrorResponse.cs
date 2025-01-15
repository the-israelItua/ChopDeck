namespace ChopDeck.Models
{
    public class ErrorResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data;
    }
}