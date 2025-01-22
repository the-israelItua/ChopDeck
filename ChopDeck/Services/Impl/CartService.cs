using ChopDeck.Dtos.Carts;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Models;
using ChopDeck.Services.Interfaces;
using ChopDeck.Mappers;
using ChopDeck.Dtos.Orders;
using Serilog;

namespace ChopDeck.Services.Impl
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepo;
        private readonly ICustomerRepository _customerRepo;
        private readonly IProductRepository _productRepo;
        public CartService(ICartRepository cartRepo, ICustomerRepository customerRepo, IProductRepository productRepo)
        {
            _cartRepo = cartRepo;
            _customerRepo = customerRepo;
            _productRepo = productRepo;
        }

        public async Task<ApiResponse<List<CartDto>>> GetCartsAsync(string userId)
        {
            try
            {
                var carts = await _cartRepo.GetAsync(userId);
                var mappedCarts = carts.Select(r => r.ToCartDto()).ToList();
                return new ApiResponse<List<CartDto>>
                {
                    Status = 200,
                    Message = "Carts fetched successfully",
                    Data = mappedCarts,
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<List<CartDto>>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<CartDto>> GetCartByIdAsync(int id, string userId)
        {
            try
            {
                var cart = await _cartRepo.GetByIdAsync(id, userId);
                if (cart == null)
                {
                    return new ApiResponse<CartDto>
                    {
                        Status = 404,
                        Message = "cart not found"
                    };
                }

                return new ApiResponse<CartDto>
                {
                    Status = 200,
                    Message = "Cart fetched successfully.",
                    Data = cart.ToCartDto()
                };

            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CartDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteCartAsync(int id, string userId)
        {
            try
            {
                var cart = await _cartRepo.DeleteAsync(id, userId);

                if (cart == null)
                {
                    return new ApiResponse<string>
                    {
                        Status = 404,
                        Message = "Cart not found."
                    };
                }

                return new ApiResponse<string>
                {
                    Status = 204,
                    Message = "Cart deleted."
                };
            }
            catch(Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<string>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<CartDto>> AddItemAsync(AddCartItemDto addCartItemDto, string userId, string userEmail)
        {
            try
            {
                var product = await _productRepo.GetByIdAsync(addCartItemDto.ProductId);

                if (product == null)
                {
                    return new ApiResponse<CartDto>
                    {
                        Status = 404,
                        Message = "Product not found."
                    };
                }

                var cart = await _cartRepo.GetByRestaurantIdAsync(product.RestaurantId, userId);

                if (cart == null)
                {
                    var customer = await _customerRepo.GetByEmailAsync(userEmail);

                    if (customer == null)
                    {
                        return new ApiResponse<CartDto>
                        {
                            Status = 404,
                            Message = "Customer not found"
                        };
                    }

                    var cartModel = new Cart
                    {
                        CustomerId = customer.Id,
                        RestaurantId = product.RestaurantId,
                    };

                    cart = await _cartRepo.CreateAsync(cartModel);
                }

                var cartItemModel = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = addCartItemDto.ProductId,
                    Quantity = addCartItemDto.Quantity,
                };

                await _cartRepo.AddItemAsync(cartItemModel);

                return new ApiResponse<CartDto>
                {
                    Status = 201,
                    Message = "Product added to cart",
                    Data = cart.ToCartDto(),
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CartDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<CartDto>> RemoveItemAsync(int cartId, int cartItemId, string userId)
        {
            try
            {
                var cart = await _cartRepo.GetByIdAsync(cartId, userId);

                if (cart == null)
                {
                    return new ApiResponse<CartDto>
                    {
                        Status = 404,
                        Message = "Cart not found."
                    };
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

                if (cartItem == null)
                {
                    return new ApiResponse <CartDto>
                    {
                        Status = 404,
                        Message = "Item not in cart."
                    };
                }

                await _cartRepo.RemoveItemAsync(cartItem);

                return new ApiResponse<CartDto>
                {
                    Status = 201,
                    Message = "Item removed from cart",
                    Data = cart.ToCartDto()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CartDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<CartDto>> UpdateItemQuantityAsync(UpdateCartItemQuantityDto quantityDto, int cartId, int cartItemId, string userId)
        {
            try
            {
                var cart = await _cartRepo.GetByIdAsync(cartId, userId);

                if (cart == null)
                {
                    return new ApiResponse<CartDto>
                    {
                        Status = 404,
                        Message = "Cart not found."
                    };
                }

                var cartItem = cart.CartItems.FirstOrDefault(ci => ci.Id == cartItemId);

                if (cartItem == null)
                {
                    return new ApiResponse<CartDto>
                    {
                        Status = 404,
                        Message = "Item not in cart."
                    };
                }

                cartItem.Quantity = quantityDto.Quantity;

                await _cartRepo.UpdateItemAsync(cartItem);

                 return new ApiResponse<CartDto>
                 {
                     Status = 201,
                     Message = "Quantity updated",
                     Data = cart.ToCartDto()
                 };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CartDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<OrderDto>> CheckoutAsync(int cartId, string userId)
        {
            try
            {
                var cart = await _cartRepo.GetByIdAsync(cartId, userId);

                if (cart == null || !cart.CartItems.Any())
                {
                    return new ApiResponse<OrderDto>
                    {
                        Status = 404,
                        Message = "Cart is empty or does not exist."
                    };
                }

                var order = await _cartRepo.CheckoutAsync(cart);

                return new ApiResponse<OrderDto>
                {
                    Status = 201,
                    Message = "Order created successfully",
                    Data = order.ToOrderDto()
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<OrderDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

    }
}
