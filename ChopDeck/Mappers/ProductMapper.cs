using ChopDeck.Dtos.Products;
using ChopDeck.Models;

namespace ChopDeck.Mappers
{
    public static class ProductMapper
    {
        public static ProductDto? ToProductDto(this Product product)
        {
            if (product == null)
            {
                return null;
            }
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                ImageUrl = product.ImageUrl,
                Price = product.Price,
                RestaurantId = product.RestaurantId,
                CreatedAt = product.CreatedAt
            };
        }

        public static Product ToProductFromCreateDto(this CreateProductDto productDto)
        {
            return new Product
            {
                Name = productDto.Name,
                Description = productDto.Description,
                ImageUrl = productDto.ImageUrl,
                Price = productDto.Price,
            };
        }
    }
}