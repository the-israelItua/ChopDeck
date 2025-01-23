using ChopDeck.Helpers;
using ChopDeck.Models;

namespace ChopDeck.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task<List<Product>> GetProductsAsync(ProductsQueryObject productsQuery);
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(Product product);
        Task<Product?> DeleteAsync(int id, string userId);
    }
}