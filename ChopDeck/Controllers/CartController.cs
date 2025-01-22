using ChopDeck.Dtos.Carts;
using ChopDeck.Helpers;
using ChopDeck.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace ChopDeck.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {

        private readonly ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Fetch all carts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCarts()
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _cartService.GetCartsAsync(userId);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Fetch cart by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        [Route("{id:int}")]
        public async Task<IActionResult> GetCartById([FromRoute] int id)
        {
             var userId = UserHelper.GetUserId(HttpContext);
             var response = await _cartService.GetCartByIdAsync(id, userId);
             return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Delete a cart
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCart([FromRoute] int id)
        {
           var userId = UserHelper.GetUserId(HttpContext);
           var response = await _cartService.DeleteCartAsync(id, userId);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Add item to cart. Creates cart if no active cart for restaurant
        /// </summary>
        /// <param name="addCartItemDto"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("add")]
        public async Task<IActionResult> AddItem([FromBody] AddCartItemDto addCartItemDto)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var userEmail = UserHelper.GetUserEmail(HttpContext);
            var response = await _cartService.AddItemAsync(addCartItemDto, userId, userEmail);   
            return ResponseHelper.HandleResponse(response); 
        }

        /// <summary>
        /// Remove item from cart
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="cartItemId"></param>
        /// <returns></returns>
        [HttpDelete("{cartId:int}/{cartItemId:int}")]
        public async Task<IActionResult> RemoveItem([FromRoute] int cartId, int cartItemId)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _cartService.RemoveItemAsync(cartId, cartItemId, userId);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Update cart item quantity
        /// </summary>
        /// <param name="cartId"></param>
        /// <param name="cartItemId"></param>
        /// <param name="quantityDto"></param>
        /// <returns></returns>
        [HttpPut("{cartId:int}/{cartItemId:int}")]
        public async Task<IActionResult> UpdateQuantity([FromRoute] int cartId, int cartItemId, [FromBody] UpdateCartItemQuantityDto quantityDto)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _cartService.UpdateItemQuantityAsync(quantityDto, cartId, cartItemId, userId);
            return ResponseHelper.HandleResponse(response); 
        }

        /// <summary>
        /// Initiate cart checkout
        /// </summary>
        /// <param name="checkoutDto"></param>
        /// <returns></returns>
        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _cartService.CheckoutAsync(checkoutDto.CartId, userId);
            return ResponseHelper.HandleResponse(response);
        }
    }
}