using MassTransit;
using Moq;
using SalesService.DTOs;
using SalesService.Models;
using SalesService.Repositories.Interfaces;
using SalesService.Services.Interfaces;
using Services.Models;
using Shared.DTOs;
using Shared.Exceptions;
using Shared.Messages;

namespace SalesService.Tests.Services
{
    public class SalesServiceTests
    {
        private readonly Mock<ISalesRepository> _salesRepositoryMock;
        private readonly Mock<IPublishEndpoint> _publishEndpointMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly ISalesService _salesService;

        public SalesServiceTests()
        {
            _salesRepositoryMock = new Mock<ISalesRepository>();
            _publishEndpointMock = new Mock<IPublishEndpoint>();
            _currentUserServiceMock = new Mock<ICurrentUserService>();

            _salesService = new SalesService.Services.SalesService(
                _salesRepositoryMock.Object,
                _publishEndpointMock.Object,
                _currentUserServiceMock.Object
            );
        }

        [Fact]
        public async Task CreateSaleAsync_ShouldReturnSaleId_WhenSaleIsCreated()
        {
            // Arrange
            var request = new SaleItemsReservedResponse
            {
                SaleId = 1,
                CustomerId = 1,
                ItemsReserved = []
            };

            _currentUserServiceMock.Setup(c => c.UserId).Returns(1);

            var createdSale = new Sale { Id = 42 };

            _salesRepositoryMock
                .Setup(r => r.CreateSaleAsync(It.IsAny<Sale>()))
                .ReturnsAsync(createdSale);

            // Act
            var result = await _salesService.CreateSaleAsync(request);

            // Assert
            Assert.Equal(42, result);
            Assert.Equal(1, request.CustomerId);

            _salesRepositoryMock.Verify(r => r.CreateSaleAsync(It.Is<Sale>(s => s.Id == 0)), Times.Once);
        }

        [Fact]
        public async Task CreateSaleAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            var request = new SaleItemsReservedResponse
            {
                SaleId = 1,
                ItemsReserved = []
            };

            _currentUserServiceMock.Setup(c => c.UserId).Returns(1);

            _salesRepositoryMock
                .Setup(r => r.CreateSaleAsync(It.IsAny<Sale>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _salesService.CreateSaleAsync(request));
            Assert.Equal("Database error", ex.Message);

            _salesRepositoryMock.Verify(r => r.CreateSaleAsync(It.IsAny<Sale>()), Times.Once);
        }

        [Fact]
        public async Task CreateSaleAsync_ShouldThrowBadRequestException_WhenRequestIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _salesService.CreateSaleAsync(null!));
        }

        [Fact]
        public async Task SendSaleAsync_ShouldThrowBadRequestException_WhenCustomerIdIsZero()
        {
            // Arrange
            var request = new SaleRequest
            {
                CustomerId = 0,
                Items = [new() { ProductId = 1, Quantity = 1 }]
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _salesService.SendSaleAsync(request));
        }

        [Fact]
        public async Task SendSaleAsync_ShouldThrowBadRequestException_WhenNoItems()
        {
            // Arrange
            var request = new SaleRequest
            {
                CustomerId = 1,
                Items = []
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _salesService.SendSaleAsync(request));
        }

        [Fact]
        public async Task SendSaleAsync_ShouldThrowBadRequestException_WhenItemProductIdIsInvalid()
        {
            // Arrange
            var request = new SaleRequest
            {
                CustomerId = 1,
                Items =
                [
                    new() { ProductId = 0, Quantity = 1 }
                ]
            };

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _salesService.SendSaleAsync(request));
        }

        [Fact]
        public async Task SendSaleAsync_ShouldPublishSale_WhenRequestIsValid()
        {
            // Arrange
            var request = new SaleRequest
            {
                CustomerId = 1,
                Items =
                [
                    new() { ProductId = 1, Quantity = 2 }
                ]
            };

            _currentUserServiceMock.Setup(c => c.UserId).Returns(42);

            // Act
            await _salesService.SendSaleAsync(request);

            // Assert
            _publishEndpointMock.Verify(p => p.Publish(It.IsAny<SaleCreated>(), It.IsAny<CancellationToken>()), Times.Once);
            Assert.Equal(1, request.CustomerId);
        }

        [Fact]
        public async Task CancelSaleAsync_ShouldThrowNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            int saleId = 1;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(42);
            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, 42)).ReturnsAsync((Sale?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.NotFoundException>(() => _salesService.CancelSaleAsync(saleId));
        }

        [Fact]
        public async Task CancelSaleAsync_ShouldThrowBadRequestException_WhenSaleIsNotPending()
        {
            // Arrange
            int saleId = 1;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(42);

            var sale = new Sale
            {
                Id = saleId,
                Status = SaleStatus.Confirmed, // não Pending
                Items = [new() { ProductId = 1, Quantity = 2 }]
            };

            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, 42)).ReturnsAsync(sale);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.BadRequestException>(() => _salesService.CancelSaleAsync(saleId));
        }

        [Fact]
        public async Task CancelSaleAsync_ShouldPublishEventsAndCancelSale_WhenSaleIsPending()
        {
            // Arrange
            int saleId = 1;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(42);

            var sale = new Sale
            {
                Id = saleId,
                Status = SaleStatus.Pending,
                Items = new List<SaleItem>
                {
                    new SaleItem { ProductId = 1, Quantity = 2 },
                    new SaleItem { ProductId = 2, Quantity = 1 }
                }
            };

            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, 42)).ReturnsAsync(sale);

            // Act
            await _salesService.CancelSaleAsync(saleId);

            // Assert
            _publishEndpointMock.Verify(p => p.Publish(It.Is<SaleCanceled>(e => e.StockItemId == 1 && e.Quantity == 2), It.IsAny<CancellationToken>()), Times.Once);
            _publishEndpointMock.Verify(p => p.Publish(It.Is<SaleCanceled>(e => e.StockItemId == 2 && e.Quantity == 1), It.IsAny<CancellationToken>()), Times.Once);

            _salesRepositoryMock.Verify(r => r.CancelSaleAsync(saleId, 42), Times.Once);
        }

        [Fact]
        public async Task CancelSaleAsync_ShouldThrowException_WhenPublishFails()
        {
            // Arrange
            int saleId = 1;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(42);

            var sale = new Sale
            {
                Id = saleId,
                Status = SaleStatus.Pending,
                Items =
                [
                    new() { ProductId = 1, Quantity = 2 }
                ]
            };

            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, 42)).ReturnsAsync(sale);
            _publishEndpointMock.Setup(p => p.Publish(It.IsAny<SaleCanceled>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Publish failed"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _salesService.CancelSaleAsync(saleId));
            Assert.Equal("Publish failed", ex.Message);
        }

        [Fact]
        public async Task GetAllSalesAsync_ShouldReturnListOfSales_WhenSalesExist()
        {
            // Arrange
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            var sales = new List<Sale>
            {
                new Sale { Id = 1, Status = SaleStatus.Pending },
                new Sale { Id = 2, Status = SaleStatus.Confirmed }
            };

            _salesRepositoryMock.Setup(r => r.GetAllAsync(userId)).ReturnsAsync(sales);

            // Act
            var result = await _salesService.GetAllSalesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.All(result, r => Assert.IsType<SaleResponse>(r));

            _salesRepositoryMock.Verify(r => r.GetAllAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllSalesAsync_ShouldReturnEmptyList_WhenNoSalesExist()
        {
            // Arrange
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _salesRepositoryMock.Setup(r => r.GetAllAsync(userId)).ReturnsAsync(new List<Sale>());

            // Act
            var result = await _salesService.GetAllSalesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _salesRepositoryMock.Verify(r => r.GetAllAsync(userId), Times.Once);
        }

        [Fact]
        public async Task GetAllSalesAsync_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _salesRepositoryMock.Setup(r => r.GetAllAsync(userId))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(_salesService.GetAllSalesAsync);
            Assert.Equal("Database error", ex.Message);
        }

        [Fact]
        public async Task GetSaleByIdAsync_ShouldReturnSaleResponse_WhenSaleExists()
        {
            // Arrange
            int saleId = 1;
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            var sale = new Sale
            {
                Id = saleId,
                Status = SaleStatus.Pending,
                Items =
                [
                    new() { ProductId = 1, Quantity = 2 }
                ]
            };

            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, userId)).ReturnsAsync(sale);

            // Act
            var result = await _salesService.GetSaleByIdAsync(saleId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(saleId, result.Id);
            Assert.All(result.Items, i => Assert.IsType<ItemResponse>(i));

            _salesRepositoryMock.Verify(r => r.GetByIdAsync(saleId, userId), Times.Once);
        }

        [Fact]
        public async Task GetSaleByIdAsync_ShouldThrowNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            int saleId = 1;
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, userId)).ReturnsAsync((Sale?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.NotFoundException>(() => _salesService.GetSaleByIdAsync(saleId));
        }

        [Fact]
        public async Task UnauthorizeSale_ShouldThrowNotFoundException_WhenSaleDoesNotExist()
        {
            // Arrange
            int saleId = 1;
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, userId)).ReturnsAsync((Sale?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exceptions.NotFoundException>(() => _salesService.UnauthorizeSale(saleId));
        }

        [Fact]
        public async Task UnauthorizeSale_ShouldCallRepository_WhenSaleExists()
        {
            // Arrange
            int saleId = 1;
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            var sale = new Sale { Id = saleId, Status = SaleStatus.Pending };
            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, userId)).ReturnsAsync(sale);

            // Act
            await _salesService.UnauthorizeSale(saleId);

            // Assert
            _salesRepositoryMock.Verify(r => r.UnauthorizeSale(sale), Times.Once);
        }

        [Fact]
        public async Task UnauthorizeSale_ShouldThrowException_WhenRepositoryFails()
        {
            // Arrange
            int saleId = 1;
            int userId = 42;
            _currentUserServiceMock.Setup(c => c.UserId).Returns(userId);

            var sale = new Sale { Id = saleId, Status = SaleStatus.Pending };
            _salesRepositoryMock.Setup(r => r.GetByIdAsync(saleId, userId)).ReturnsAsync(sale);
            _salesRepositoryMock.Setup(r => r.UnauthorizeSale(sale)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _salesService.UnauthorizeSale(saleId));
            Assert.Equal("Database error", ex.Message);
        }
    }
    
}