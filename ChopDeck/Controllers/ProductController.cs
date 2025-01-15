using System.Security.Claims;
using ChopDeck.Dtos.Products;
using ChopDeck.helpers;
using ChopDeck.Interfaces;
using ChopDeck.Mappers;
using ChopDeck.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChopDeck.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/product")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly IRestaurantRepository _restaurantRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductsController(IProductRepository productRepo, IRestaurantRepository restaurantRepo, UserManager<ApplicationUser> userManager)
        {
            _productRepo = productRepo;
            _restaurantRepo = restaurantRepo;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductsQueryObject productsQuery)
        {
            var products = await _productRepo.GetProductsAsync(productsQuery);
            var mappedProducts = products.Select(s => s.ToProductDto()).ToList();
            return Ok(new ApiResponse<List<ProductDto>>
            {
                Status = 200,
                Message = "Products fetched successfully",
                Data = mappedProducts
            });
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductByID([FromRoute] int id)
        {
            var product = await _productRepo.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Product not found"
                });
            }

            return Ok(new ApiResponse<ProductDto>
            {
                Status = 200,
                Message = "Product fetched successfully.",
                Data = product.ToProductDto()
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var restaurant = await _restaurantRepo.GetByUserIdAsync(userId);
            if (restaurant == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "You do not have permission to perform this operation.",
                    Data = userId
                });
            }
            try
            {


                var productModel = productDto.ToProductFromCreateDto();

                productModel.RestaurantId = restaurant.Id;

                await _productRepo.CreateAsync(productModel);
                return CreatedAtAction(nameof(GetProductByID), new { id = productModel.Id }, productModel.ToProductDto());
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ErrorResponse<Exception>
                {
                    Status = 500,
                    Message = "An unexpected error occurred.",
                    Data = ex
                });
            }
        }

        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, UpdateProductDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var restaurant = await _restaurantRepo.GetByUserIdAsync(userId);
            if (restaurant == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "You do not have permission to perform this operation."
                });
            }

            var product = await _productRepo.GetByIdAsync(id);
            if (product == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Product not found"
                });
            }
            if (!string.IsNullOrWhiteSpace(updateDto.Name))
            {
                product.Name = updateDto.Name;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Description))
            {
                product.Description = updateDto.Description;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.ImageUrl))
            {
                product.ImageUrl = updateDto.ImageUrl;
            }

            if (updateDto.Price.HasValue && updateDto.Price.Value > 0)
            {
                product.Price = updateDto.Price.Value;
            }
            await _productRepo.UpdateAsync(product);
            return Ok(new ApiResponse<ProductDto>
            {
                Status = 200,
                Message = "Product updated successfully.",
                Data = product.ToProductDto()
            });

        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var product = await _productRepo.DeleteAsync(id);

            if (product == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Product not found."
                });
            }

            return NoContent();
        }
    }
}