using Newtonsoft.Json;

namespace ChopDeck.Models
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Token { get; set; }
    }
}