using Azure;
using ChopDeck.Services.Interfaces;
using ChopDeck.Models;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChopDeck.Services.Impl
{
    public class PaystackService : IPaystackService
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackSettings _paystackSettings;

        public PaystackService(HttpClient httpClient, IOptions<AppSettings> appSettings)
        {
            _paystackSettings = appSettings.Value.Paystack;
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {appSettings.Value.Paystack.SecretKey}");
        }

        public async Task<string> InitializeTransactionAsync(string email, decimal amount, string callbackUrl)
        {
                var payload = new
            {
                email,
                amount = amount * 100,
                callback_url = callbackUrl
            };

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"{_paystackSettings.BaseUrl}/transaction/initialize", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PaystackResponse>(responseBody);
                return result.Data.AuthorizationUrl;
              }

                return $"Paystack Error: {await response.Content.ReadAsStringAsync()}";
            }


        public async Task<bool> VerifyTransactionAsync(string reference)
        {
            var response = await _httpClient.GetAsync($"{_paystackSettings.BaseUrl}/transaction/verify/{reference}");

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<PaystackResponse>(responseBody);
                return result.Status;
            }

            return false;
        }
    }
}
