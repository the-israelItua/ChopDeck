namespace ChopDeck.Models
{
    public class PaystackResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public PaystackData Data { get; set; } = new PaystackData();
    }

    public class PaystackData
    {
        public string AuthorizationUrl { get; set; } = string.Empty;
        public string AccessCode { get; set; } = string.Empty;
        public string Reference { get; set; } = string.Empty;
    }
}
