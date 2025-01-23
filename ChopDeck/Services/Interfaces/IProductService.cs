using ChopDeck.Dtos.Carts;
using ChopDeck.Dtos.Products;
using ChopDeck.Helpers;
using ChopDeck.Models;

namespace ChopDeck.Services.Interfaces
{
    public interface IProductService
    {
        Task<ApiResponse<List<ProductDto>>> GetProductsAsync(ProductsQueryObject productsQuery);
        Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id);
        Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto productDto, string userId);
        Task<ApiResponse<ProductDto>> UpdateProductAsync(int id, UpdateProductDto productDto, string userId);
        Task<ApiResponse<ProductDto>> DeleteProductAsync(int id, string userId);
    }
}
