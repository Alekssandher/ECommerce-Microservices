using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Shared.ModelViews;
using StockService.Dtos;
using StockService.Services;

namespace StockService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(BadRequest), StatusCodes.Status400BadRequest, "application/problem+json")]
    [ProducesResponseType(typeof(InternalError), StatusCodes.Status500InternalServerError, "application/problem+json")]
    public class StockController : ControllerBase
    {
        private readonly IStockManagementService _stockService;
        
        public StockController(IStockManagementService stockService)
        {
            _stockService = stockService;
        }

        [HttpGet("{productId}")]
        public async Task<IActionResult> GetStockById([FromRoute] int productId)
        {
            var stock = await _stockService.GetStockByIdAsync(productId);

            return Ok(new OkResponse<StockResponse>(string.Empty, string.Empty, stock));
        }   
        [HttpGet("{productId}/available")]
        public async Task<IActionResult> GetAvailableStock([FromRoute] int productId)
        {
            var quantity = await _stockService.GetAvailableStockAsync(productId);
            return Ok(new OkResponse<int>(string.Empty, string.Empty, quantity));
        }

        [HttpPost("{productId}/reserve/{quantity}")]
        public async Task<IActionResult> ReserveStock([FromRoute] int productId, [FromRoute] int quantity)
        {
            await _stockService.ReserveStockAsync(productId, quantity);
            return NoContent();
        }

        [HttpPost("{productId}/release/{quantity}")]
        public async Task<IActionResult> ReleaseStock([FromRoute] int productId, [FromRoute] int quantity)
        {
            await _stockService.ReleaseStockAsync(productId, quantity);
            return NoContent();
        }

        [HttpDelete("{productId}/remove/{quantity}")]
        public async Task<IActionResult> RemoveStock([FromRoute] int productId, [FromRoute] int quantity)
        {
            await _stockService.RemoveStockAsync(productId, quantity);
            return NoContent();
        }
    }
}