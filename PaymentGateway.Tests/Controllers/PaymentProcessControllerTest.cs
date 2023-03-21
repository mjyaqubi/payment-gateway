using System;
using System.Collections.Generic;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGateway.ApplicationService.Models;
using PaymentGateway.ApplicationService.PaymentHistory.Dto;
using PaymentGateway.ApplicationService.PaymentHistory.Services;
using PaymentGateway.ApplicationService.PaymentProcess.Dto;
using PaymentGateway.ApplicationService.PaymentProcess.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using PaymentGateway.WebApi.Controllers;
using RichardSzalay.MockHttp;

namespace PaymentGateway.Tests.Controllers
{
	public class PaymentProcessControllerTest
    {
        private readonly Guid merchantId;
        private readonly Guid merchantOrderId;
        private readonly string cardNumber;
        private readonly PaymentGatewayDbContext processServiceDbContext;
        private readonly PaymentGatewayDbContext historyServiceDbContext;
        private readonly PaymentProcessController controller;

        public PaymentProcessControllerTest()
		{
            merchantId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            merchantOrderId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            cardNumber = "1234567890123456";

            var processServiceDbOptions = new DbContextOptionsBuilder<PaymentGatewayDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentTransaction_Controller_ProcessService");

            processServiceDbContext = new PaymentGatewayDbContext(processServiceDbOptions.Options);

            var dbOptionsForHistoryService = new DbContextOptionsBuilder<PaymentGatewayDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentTransaction_Controller_HistoryService");

            historyServiceDbContext = new PaymentGatewayDbContext(dbOptionsForHistoryService.Options);
            historyServiceDbContext.Database.EnsureDeleted();
            historyServiceDbContext.PaymentTransaction.Add(new PaymentTransaction
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
            historyServiceDbContext.PaymentTransaction.Add(new PaymentTransaction
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
            historyServiceDbContext.PaymentTransaction.Add(new PaymentTransaction
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
            historyServiceDbContext.SaveChanges();

            var bankResponse = "{\"id\":\"12345678-1234-1234-1234-123456789012\"," +
                "\"paymentGatewayOrderId\":\"12345678-1234-1234-1234-123456789012\"," +
                "\"currency\":\"SEK\",\"amount\":1000,\"status\":\"completed\"}";

            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When("http://localhost/api/*/process")
                .Respond("application/json", bankResponse);

            IOptions<AcquiringBankSettings> settings = Options.Create(new AcquiringBankSettings { BaseUrl = "http://localhost/", PaymentGatewayID = "12345678-1234-1234-1234-123456789012" });

            var paymentProcessService = new PaymentProcessService(processServiceDbContext, settings, mockHttp.ToHttpClient());
            var paymentHistoryService = new PaymentHistoryService(historyServiceDbContext);
            controller = new PaymentProcessController(paymentProcessService, paymentHistoryService);
        }

        [Fact]
        public async void GetHistory()
        {
            // Arrage
            var request = new PaymentHistoryRequest { };

            // Act
            var result = await controller.IndexAsync(merchantId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentTransaction>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async void GetHistory_Filter_By_CardNumber()
        {
            // Arrage
            var request = new PaymentHistoryRequest
            {
                CardNumber = cardNumber
            };

            // Act
            var result = await controller.IndexAsync(merchantId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentTransaction>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async void GetHistory_Filter_By_MerchantOrderId()
        {
            // Arrage
            var request = new PaymentHistoryRequest
            {
                MerchantOrderId = merchantOrderId
            };

            // Act
            var result = await controller.IndexAsync(merchantId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<PaymentTransaction>>(okResult.Value);
            Assert.Single(returnValue.ToList());
        }

        [Fact]
        public async void GetHistory_DataValidationError()
        {
            // Arrage
            var request = new PaymentHistoryRequest {
                CardNumber = ""
            };
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.IndexAsync(merchantId, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(3, historyServiceDbContext.PaymentTransaction.Count());
        }

        [Fact]
        public async void Process()
        {
            // Arrage
            var request = new PaymentProcessRequest
            {
                MerchantOrderId = merchantOrderId,
                CardNumber = cardNumber,
                ExpiryMonth = "10",
                ExpiryYear = "25",
                CVV = "123",
                Currency = "SEK",
                Amount = 1000,
            };

            // Act
            var result = await controller.ProcessAsync(merchantId, request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<PaymentProcessResponse>(okResult.Value);
            Assert.IsType<PaymentProcessResponse>(returnValue);
            Assert.Equal(returnValue.Status.ToString(), TransactionStatus.Completed.ToString());
            Assert.Equal(1, processServiceDbContext.PaymentTransaction.Count());
        }

        [Fact]
        public async void Process_DataValidationError()
        {
            // Arrage
            var request = new PaymentProcessRequest
            {
                MerchantOrderId = merchantOrderId,
                CardNumber = cardNumber,
                ExpiryMonth = "20",
                ExpiryYear = "25",
                CVV = "123",
                Currency = "SEK",
                Amount = 1000,
            };
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = await controller.ProcessAsync(merchantId, request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(0, processServiceDbContext.PaymentTransaction.Count());
        }
    }
}

