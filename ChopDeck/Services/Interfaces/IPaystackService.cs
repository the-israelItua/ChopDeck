namespace ChopDeck.Services.Interfaces
{
    public interface IPaystackService
    {
      Task<string> InitializeTransactionAsync(string email, decimal amount, string callbackUrl);
      Task<bool> VerifyTransactionAsync(string reference);
    }
}
