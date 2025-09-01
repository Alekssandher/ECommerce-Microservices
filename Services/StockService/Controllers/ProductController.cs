using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockService.Dtos;
using StockService.Services;
using Shared.ModelViews;

namespace StockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _productService.GetAllProductsAsync();
            return Ok(new OkResponse<List<ProductResponse>>(string.Empty, string.Empty, products));
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById([FromRoute] int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            return Ok(new OkResponse<ProductResponse>(string.Empty, string.Empty, product));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
        {
            await _productService.CreateProductAsync(request);
            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductRequest request)
        {
            await _productService.UpdateProductAsync(request);
            return NoContent();
        }

        [HttpDelete("{productId}/{quantity}")]
        public async Task<IActionResult> RemoveProductQuantity([FromRoute] int productId, [FromRoute] int quantity)
        {
            await _productService.RemoveProductQuantityAsync(productId, quantity);
            return NoContent();
        }
    }
}