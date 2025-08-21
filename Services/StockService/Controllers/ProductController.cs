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
            var res = await _productService.GetAllProducts();

            return Ok(new OkResponse<List<ProductResponse>>(string.Empty, string.Empty, res));
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductById([FromRoute] int productId)
        {
            var res = await _productService.GetProductById(productId);

            return Ok(new OkResponse<ProductResponse>(string.Empty, string.Empty, res));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest productRequest)
        {
            await _productService.CreateProduct(productRequest);

            return Created();
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct([FromBody] ProductUpdateRequest productUpdateRequest)
        {
            await _productService.UpdateProduct(productUpdateRequest);

            return NoContent();
        }

        [HttpDelete("{productId}/{quantity}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int productId, [FromRoute] int quantity)
        {
            await _productService.DeleteProduct(productId, quantity);

            return NoContent();
        }
    }
}