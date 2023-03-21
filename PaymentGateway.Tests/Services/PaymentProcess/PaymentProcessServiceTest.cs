using System;
using System.ComponentModel.DataAnnotations;
using Azure.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGateway.ApplicationService.Models;
using PaymentGateway.ApplicationService.PaymentHistory.Services;
using PaymentGateway.ApplicationService.PaymentProcess.Dto;
using PaymentGateway.ApplicationService.PaymentProcess.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;
using RichardSzalay.MockHttp;
using Xunit.Sdk;

namespace PaymentGateway.Tests.Services.PaymentProcess
{
    public class PaymentProcessServiceTest
    {
        private readonly Guid merchantId;
        private readonly Guid merchantOrderId;
        private readonly string cardNumber;
        private readonly PaymentProcessRequest request;

        public PaymentProcessServiceTest()
        {
            merchantId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            merchantOrderId = Guid.Parse("12345678-1234-1234-1234-123456789012");
            cardNumber = "1234567890123456";

            request = new PaymentProcessRequest
            {
                MerchantOrderId = merchantOrderId,
                CardNumber = cardNumber,
                ExpiryMonth = "10",
                ExpiryYear = "25",
                CVV = "123",
                Currency = "SEK",
                Amount = 1000,
            };
        }

        [Fact]
        public async void Process_Completed()
        {
            // Arrange
            var dbOptions = new DbContextOptionsBuilder<PaymentGatewayDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentTransaction_ProcessService_Completed");
            var dbContext = new PaymentGatewayDbContext(dbOptions.Options);

            var mockHttp = new MockHttpMessageHandler();
            var bankResponse = "{\"id\":\"12345678-1234-1234-1234-123456789012\"," +
                "\"paymentGatewayOrderId\":\"12345678-1234-1234-1234-123456789012\"," +
                "\"currency\":\"SEK\",\"amount\":1000,\"status\":\"completed\"}";
            mockHttp.When("http://localhost/api/*/process")
                .Respond("application/json", bankResponse);

            IOptions<AcquiringBankSettings> settings = Options.Create(new AcquiringBankSettings { BaseUrl = "http://localhost/", PaymentGatewayID = "12345678-1234-1234-1234-123456789012" });
            var service = new PaymentProcessService(dbContext, settings, mockHttp.ToHttpClient());

            // Act
            PaymentProcessResponse response = await service.ProcessAsync(merchantId, request);

            // Assert
            Assert.IsType<PaymentProcessResponse>(response);
            Assert.Equal(response.Status.ToString(), TransactionStatus.Completed.ToString());
            Assert.Equal(1, dbContext.PaymentTransaction.Count());
        }

        [Fact]
        public async void Process_Declined()
        {
            // Arrange
            var dbOptions = new DbContextOptionsBuilder<PaymentGatewayDbContext>()
                .UseInMemoryDatabase(databaseName: "PaymentTransaction_ProcessService_Declined");
            var dbContext = new PaymentGatewayDbContext(dbOptions.Options);

            var mockHttp = new MockHttpMessageHandler();
            var bankResponse = "{\"id\":\"12345678-1234-1234-1234-123456789012\"," +
                "\"paymentGatewayOrderId\":\"12345678-1234-1234-1234-123456789012\"," +
                "\"currency\":\"SEK\",\"amount\":1000,\"status\":\"declined\",\"declineReason\":\"refused\"}";
            mockHttp.When("http://localhost/api/*/process")
                .Respond("application/json", bankResponse);

            IOptions<AcquiringBankSettings> settings = Options.Create(new AcquiringBankSettings { BaseUrl = "http://localhost/", PaymentGatewayID = "12345678-1234-1234-1234-123456789012" });
            var service = new PaymentProcessService(dbContext, settings, mockHttp.ToHttpClient());

            // Act
            PaymentProcessResponse response = await service.ProcessAsync(merchantId, request);

            // Assert
            Assert.IsType<PaymentProcessResponse>(response);
            Assert.Equal(response.Status.ToString(), TransactionStatus.Declined.ToString());
            Assert.Equal(response.DeclineReason.ToString(), TransactionDeclineReason.Refused.ToString());
            Assert.Equal(1, dbContext.PaymentTransaction.Count());
        }
    }
}