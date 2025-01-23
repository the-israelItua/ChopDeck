using ChopDeck.Dtos.Products;
using ChopDeck.Helpers;
using ChopDeck.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChopDeck.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/product")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Get products
        /// </summary>
        /// <param name="productsQuery"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] ProductsQueryObject productsQuery)
        {
          var response = await _productService.GetProductsAsync(productsQuery); 
           return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Get product by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProductByID([FromRoute] int id)
        {
            var response = await _productService.GetProductByIdAsync(id);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Add a new product
        /// </summary>
        /// <param name="productDto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto productDto)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _productService.CreateProductAsync(productDto, userId);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Update product information
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPatch]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, UpdateProductDto updateDto)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _productService.UpdateProductAsync(id, updateDto, userId);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Delete product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _productService.DeleteProductAsync(id, userId);
            return ResponseHelper.HandleResponse(response);   
        }
    }
}