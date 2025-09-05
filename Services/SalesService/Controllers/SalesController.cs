using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Mvc;
using SalesService.DTOs;
using SalesService.Services.Interfaces;
using Shared.ModelViews;

namespace SalesService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _salesService;
        private readonly IPublishEndpoint _publishEndpoint;
        public SalesController(ISalesService salesService, IPublishEndpoint publishEndpoint)
        {
            _salesService = salesService;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {            
            var sales = await _salesService.GetAllSalesAsync();
            return Ok(new OkResponse<List<SaleResponse>>(string.Empty, string.Empty, sales));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
        
            var usid = User.FindFirst("UserId")?.Value;

            var sale = await _salesService.GetSaleByIdAsync(id);
            return Ok(new OkResponse<SaleResponse>(string.Empty, string.Empty, sale));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaleRequest request)
        {
            await _salesService.SendSaleAsync(request);
            
            return Accepted(new
            {
                Message = "Sale created and pending stock reservation."
            });
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> ConfirmSale([FromRoute] int id)
        {
            await _salesService.ConfirmSaleAsync(id);

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> CancelSale([FromRoute] int id)
        {
            
            await _salesService.CancelSaleAsync(id);

            return NoContent();
        }
    }
}