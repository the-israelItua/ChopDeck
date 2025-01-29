namespace ChopDeck.Models
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; } = new();
        public Logging Logging { get; set; } = new();
        public string AllowedHosts { get; set; } = string.Empty;
        public JwtSettings JWT { get; set; } = new();
        public PaystackSettings Paystack { get; set; } = new();
    }

    public class ConnectionStrings
    {
        public string DefaultConnection { get; set; } = string.Empty;
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; } = new();
    }

    public class LogLevel
    {
        public string Default { get; set; } = string.Empty;
        public string MicrosoftAspNetCore { get; set; } = string.Empty;
    }

    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string SigningKey { get; set; } = string.Empty;
    }

    public class PaystackSettings
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string SecretKey { get; set; } = string.Empty;
    }

}
