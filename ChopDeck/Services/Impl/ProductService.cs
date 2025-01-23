﻿using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos.Products;
using ChopDeck.Helpers;
using ChopDeck.Mappers;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace ChopDeck.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepo;
        private readonly IRestaurantRepository _restaurantRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductService(IProductRepository productRepo, IRestaurantRepository restaurantRepo, UserManager<ApplicationUser> userManager)
        {
            _productRepo = productRepo;
            _restaurantRepo = restaurantRepo;
            _userManager = userManager;
        }
        public async Task<ApiResponse<ProductDto>> CreateProductAsync(CreateProductDto productDto, string userId)
        {
            try
            {
                var restaurant = await _restaurantRepo.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return new ApiResponse<ProductDto>
                    {
                        Status = 401,
                        Message = "You do not have permission to perform this operation.",
                    };
                }

                var productModel = productDto.ToProductFromCreateDto();

                productModel.RestaurantId = restaurant.Id;

                await _productRepo.CreateAsync(productModel);
                return new ApiResponse<ProductDto>
                {
                    Status = 201,
                    Message = "Product created successfully.",
                    Data = productModel.ToProductDto()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<ProductDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<ProductDto>> DeleteProductAsync(int id, string userId)
        {
            try
            {
                var product = await _productRepo.DeleteAsync(id, userId);

                if (product == null)
                {
                    return new ApiResponse<ProductDto>
                    {
                        Status = 404,
                        Message = "Product not found."
                    };
                }

               return new ApiResponse<ProductDto>
                {
                    Status = 204,
                    Message = "Product deleted successfully."
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<ProductDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<ProductDto>> GetProductByIdAsync(int id)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(id);

                if (product == null)
                {
                    return new ApiResponse<ProductDto>
                    {
                        Status = 404,
                        Message = "Product not found"
                    };
                }

                return new ApiResponse<ProductDto>
                {
                    Status = 200,
                    Message = "Product fetched successfully.",
                    Data = product.ToProductDto()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<ProductDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<List<ProductDto>>> GetProductsAsync(ProductsQueryObject productsQuery)
        {
            try
            {
                var products = await _productRepo.GetProductsAsync(productsQuery);
                var mappedProducts = products.Select(s => s.ToProductDto()).ToList();
                return new ApiResponse<List<ProductDto>>
                {
                    Status = 200,
                    Message = "Products fetched successfully",
                    Data = mappedProducts
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<List<ProductDto>>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<ProductDto>> UpdateProductAsync(int id, UpdateProductDto updateDto, string userId)
        {
            try
            {
                var restaurant = await _restaurantRepo.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return new ApiResponse<ProductDto>
                    {
                        Status = 401,
                        Message = "You do not have permission to perform this operation."
                    };
                }

                var product = await _productRepo.GetByIdAsync(id);
                if (product == null)
                {
                    return new ApiResponse<ProductDto>
                    {
                        Status = 404,
                        Message = "Product not found"
                    };
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
                return new ApiResponse<ProductDto>
                {
                    Status = 200,
                    Message = "Product updated successfully.",
                    Data = product.ToProductDto()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<ProductDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }
    }
}
