using MassTransit;
using Microsoft.Extensions.Configuration.UserSecrets;
using SalesService.DTOs;
using SalesService.Mappers;
using SalesService.Models;
using SalesService.Repositories.Interfaces;
using SalesService.Services.Interfaces;
using Serilog;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Messages;
using Shared.ModelViews;

namespace SalesService.Services
{
    internal class SalesService : ISalesService
    {
        private readonly ISalesRepository _salesRepository;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ICurrentUserService _currentUserService;

        public SalesService(ISalesRepository salesRepository, IPublishEndpoint publishEndpoint, ILogger<SalesService> logger, ICurrentUserService currentUserService)
        {
            _salesRepository = salesRepository;
            _publishEndpoint = publishEndpoint;
            _currentUserService = currentUserService;

        }

        public async Task<int> CreateSaleAsync(SaleItemsReservedResponse request)
        {
            Log.Information("Starting sale creation - CustomerId: {CustomerId}, SaleId: {SaleId}",
                request.CustomerId, request.SaleId);

            if (request == null)
            {
                Log.Warning("Invalid SaleItemsReservedResponse received - CustomerId: {CustomerId}, SaleId: {SaleId}",
                    request?.CustomerId, request?.SaleId );
                throw new Exceptions.BadRequestException("Invalid sale request data");
            }

            var usid = _currentUserService.UserId;
            request.CustomerId = usid;

            try
            {
                var sale = await _salesRepository.CreateSaleAsync(request.ToModel());
                Log.Information("Sale created successfully - SaleId: {SaleId}, CustomerId: {CustomerId}, UserId: {UserId}",
                    sale.Id, request.CustomerId, usid);
                return sale.Id;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create sale - SaleId: {SaleId}, CustomerId: {CustomerId}, UserId: {UserId}, Error: {Error}",
                    request.SaleId, request.CustomerId, usid, ex.Message);
                throw;
            }
        }

        public async Task SendSaleAsync(SaleRequest request)
        {
            Log.Information("Starting sale submission - CustomerId: {CustomerId}, ItemCount: {ItemCount}",
                request.CustomerId, request.Items.Count);

            if (request.CustomerId <= 0)
            {
                Log.Warning("Invalid CustomerId in sale request - CustomerId: {CustomerId}", request.CustomerId);
                throw new Exceptions.BadRequestException("Customer ID must be greater than zero");
            }

            if (request.Items.Count <= 0)
            {
                Log.Warning("Sale request with no items - CustomerId: {CustomerId}", request.CustomerId);
                throw new Exceptions.BadRequestException("Sale must contain at least one item");
            }

            foreach (var item in request.Items)
            {
                if (item.ProductId <= 0)
                {
                    Log.Warning("Invalid ProductId in sale request - CustomerId: {CustomerId}, ProductId: {ProductId}",
                        request.CustomerId, item.ProductId);
                    throw new Exceptions.BadRequestException($"Invalid Product ID: {item.ProductId}");
                }
            }

            var sale = request.ToSaleModel();
            var usid = _currentUserService.UserId;
            sale.CustomerId = usid;

            try
            {
                await _publishEndpoint.Publish(sale.ToSaleCreated());
                Log.Information("Sale published successfully - CustomerId: {CustomerId}, UserId: {UserId}",
                    sale.CustomerId, usid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to publish sale - CustomerId: {CustomerId}, UserId: {UserId}, Error: {Error}",
                    sale.CustomerId, usid, ex.Message);
                throw;
            }
        }

        public async Task CancelSaleAsync(int saleId)
        {
            var usid = _currentUserService.UserId;
            Log.Information("Starting sale cancellation - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);

            var sale = await _salesRepository.GetByIdAsync(saleId, usid)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            if (sale.Status != SaleStatus.Pending)
            {
                Log.Warning("Attempt to cancel non-pending sale - SaleId: {SaleId}, Status: {Status}, UserId: {UserId}",
                    saleId, sale.Status, usid);
                throw new Exceptions.BadRequestException("You Can Only Cancel Pending Sales");
            }

            try
            {
                foreach (var item in sale.Items)
                {
                    await _publishEndpoint.Publish(new SaleCanceled
                    {
                        StockItemId = item.ProductId,
                        Quantity = item.Quantity
                    });
                    Log.Information("Published SaleCanceled event - SaleId: {SaleId}, ProductId: {ProductId}, Quantity: {Quantity}, UserId: {UserId}",
                        saleId, item.ProductId, item.Quantity, usid);
                }

                await _salesRepository.CancelSaleAsync(sale.Id, usid);
                Log.Information("Sale canceled successfully - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to cancel sale - SaleId: {SaleId}, UserId: {UserId}, Error: {Error}",
                    saleId, usid, ex.Message);
                throw;
            }
        }

        public async Task ConfirmSaleAsync(int saleId)
        {
            var usid = _currentUserService.UserId;
            Log.Information("Starting sale confirmation - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);

            var sale = await _salesRepository.GetByIdAsync(saleId, usid)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            if (sale.Status != SaleStatus.Pending)
            {
                Log.Warning("Attempt to confirm non-pending sale - SaleId: {SaleId}, Status: {Status}, UserId: {UserId}",
                    saleId, sale.Status, usid);
                throw new Exceptions.BadRequestException("You Can Only Confirm Pending Sales");
            }

            try
            {
                await _salesRepository.ConfirmSaleAsync(sale);
                Log.Information("Sale confirmed successfully - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);

                foreach (var item in sale.Items)
                {
                    await _publishEndpoint.Publish(new SaleConfirmed(
                        item.ProductId,
                        item.Quantity
                    ));
                    Log.Information("Published SaleConfirmed event - SaleId: {SaleId}, ProductId: {ProductId}, Quantity: {Quantity}, UserId: {UserId}",
                        saleId, item.ProductId, item.Quantity, usid);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to confirm sale - SaleId: {SaleId}, UserId: {UserId}, Error: {Error}",
                    saleId, usid, ex.Message);
                throw;
            }
        }

        public async Task<List<SaleResponse>> GetAllSalesAsync()
        {
            var usid = _currentUserService.UserId;
            Log.Information("Retrieving all sales - UserId: {UserId}", usid);

            try
            {
                var sales = await _salesRepository.GetAllAsync(usid);
                Log.Information("Retrieved {SaleCount} sales - UserId: {UserId}", sales.Count, usid);
                return sales.ToSaleResponseList();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to retrieve sales - UserId: {UserId}, Error: {Error}", usid, ex.Message);
                throw;
            }
        }

        public async Task<SaleResponse> GetSaleByIdAsync(int saleId)
        {
            var usid = _currentUserService.UserId;
            Log.Information("Retrieving sale by ID - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);

            var sale = await _salesRepository.GetByIdAsync(saleId, usid)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            Log.Information("Sale retrieved successfully - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);
            return sale.ToSaleResponse();
        }

        public async Task UnauthorizeSale(int saleId)
        {
            var usid = _currentUserService.UserId;
            Log.Information("Starting sale unauthorization - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);

            var sale = await _salesRepository.GetByIdAsync(saleId, usid)
                ?? throw new Exceptions.NotFoundException($"Sale with ID: {saleId} Not Found.");

            try
            {
                await _salesRepository.UnauthorizeSale(sale);
                Log.Information("Sale unauthorized successfully - SaleId: {SaleId}, UserId: {UserId}", saleId, usid);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to unauthorize sale - SaleId: {SaleId}, UserId: {UserId}, Error: {Error}",
                    saleId, usid, ex.Message);
                throw;
            }
        }
    }
}