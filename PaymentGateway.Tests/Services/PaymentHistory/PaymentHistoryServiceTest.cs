using System;
using Microsoft.EntityFrameworkCore.InMemory;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.ApplicationService;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.ApplicationService.PaymentHistory.Services;
using PaymentGateway.ApplicationService.PaymentHistory.Dto;
using System.Collections.Generic;

namespace PaymentGateway.Tests.Services.PaymentHistory
{
	public class PaymentHistoryServiceTest
	{
        private readonly Guid merchantId;
        private readonly Guid merchantOrderId;
        private readonly string cardNumber;
        private readonly DbContextOptions<PaymentGatewayDbContext> dbOptions;
        private readonly PaymentHistoryService service;

        public PaymentHistoryServiceTest()
		{
            merchantId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            merchantOrderId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            cardNumber = "1234567890123456";

            dbOptions = new DbContextOptionsBuilder<PaymentGatewayDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentTransaction_HistoryService")
                .Options;


            var dbContext = new PaymentGatewayDbContext(dbOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.PaymentTransaction.Add(new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                MerchantId = merchantId,
                MerchantOrderId = merchantOrderId,
                BankPaymentId = Guid.NewGuid(),
                CardNumber = cardNumber,
                MaskedCardNumber = "************3456",
                Currency = "SEK",
                Amount = 1000,
                Status = TransactionStatus.Completed,
                DeclineReason = null,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.PaymentTransaction.Add(new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                MerchantId = merchantId,
                MerchantOrderId = Guid.NewGuid(),
                BankPaymentId = Guid.NewGuid(),
                CardNumber = cardNumber,
                MaskedCardNumber = "************3456",
                Currency = "SEK",
                Amount = 1000,
                Status = TransactionStatus.Declined,
                DeclineReason = TransactionDeclineReason.Refused,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.PaymentTransaction.Add(new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                MerchantId = merchantId,
                MerchantOrderId = Guid.NewGuid(),
                BankPaymentId = Guid.NewGuid(),
                CardNumber = cardNumber,
                MaskedCardNumber = "************3456",
                Currency = "SEK",
                Amount = 1000,
                Status = TransactionStatus.Declined,
                DeclineReason = TransactionDeclineReason.Blocked_Card,
                CreatedAt = DateTime.UtcNow,
            });
            dbContext.SaveChanges();
            service = new PaymentHistoryService(dbContext);
        }

		[Fact]
		public async void GetHistory()
        {
            // Arrange
            PaymentHistoryRequest request = new PaymentHistoryRequest {};

            // Act
            IEnumerable<PaymentTransaction> result = await service.GetHistoryAsync(merchantId, request);

            // Assert
            Assert.Equal(3, result.ToList().Count);
        }

        [Fact]
        public async void GetHistory_Filter_By_CardNumber()
        {
            // Arrange
            PaymentHistoryRequest request = new PaymentHistoryRequest {
                CardNumber = cardNumber
            };

            // Act
            IEnumerable<PaymentTransaction> result = (await service.GetHistoryAsync(merchantId, request)).ToList();

            // Assert
            Assert.Equal(3, result.ToList().Count);
        }

        [Fact]
        public async void GetHistory_Filter_By_MerchantOrderId()
        {
            // Arrange
            PaymentHistoryRequest request = new PaymentHistoryRequest
            {
                MerchantOrderId = merchantOrderId
            };

            // Act
            IEnumerable<PaymentTransaction> result = await service.GetHistoryAsync(merchantId, request);

            // Assert
            Assert.Single(result.ToList());
        }
    }
}