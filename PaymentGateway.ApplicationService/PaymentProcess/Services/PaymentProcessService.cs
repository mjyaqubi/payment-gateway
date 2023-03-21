using System;
using PaymentGateway.ApplicationService.PaymentProcess.Dto;
using PaymentGateway.Domain.Entities;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using PaymentGateway.ApplicationService.Models;
using PaymentGateway.Domain;

namespace PaymentGateway.ApplicationService.PaymentProcess.Services
{
	public class PaymentProcessService: IPaymentProcessService
	{
        private readonly AcquiringBankSettings settings;
        private readonly PaymentGatewayDbContext dbContext;
        private readonly HttpClient httpClient;

        public PaymentProcessService(PaymentGatewayDbContext dbContext, IOptions<AcquiringBankSettings> settings) {
            this.settings = settings.Value;
            this.dbContext = dbContext;
            this.httpClient = new HttpClient();
        }

        public PaymentProcessService(PaymentGatewayDbContext dbContext, IOptions<AcquiringBankSettings> settings, HttpClient httpClient)
        {
            this.settings = settings.Value;
            this.dbContext = dbContext;
            this.httpClient = httpClient;
        }

        public async Task<PaymentProcessResponse> ProcessAsync(Guid merchantId, PaymentProcessRequest request)
        {
            // validation: auto using attributes
            // call api
            var Id = Guid.NewGuid();
            var bankResponse = await httpClient.PostAsJsonAsync(settings.BaseUrl + "api/"+ settings.PaymentGatewayID +"/process", new
            {
                PaymentGatewayOrderId = Id,
                CardNumber = request.CardNumber,
                ExpiryMonth = request.ExpiryMonth,
                ExpiryYear = request.ExpiryYear,
                Cvv = request.CVV,
                Currency = request.Currency,
                Amount = request.Amount,
            });

            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new JsonStringEnumConverter());
            jsonOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            var bankResult = await bankResponse.Content.ReadFromJsonAsync<BankPaymentResponse>(jsonOptions);

            // save to db
            var transaction = new PaymentTransaction
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                DeclineReason = bankResult.DeclineReason.HasValue ? (TransactionDeclineReason)bankResult.DeclineReason : null,
                CardNumber = request.CardNumber,
                CreatedAt = DateTime.UtcNow,
                Currency = request.Currency,
                MaskedCardNumber = $"****-{request.CardNumber.Substring(12)}",
                MerchantId = merchantId,
                MerchantOrderId = request.MerchantOrderId,
                BankPaymentId = bankResult.Id,
                Status = bankResult.Status == BankPaymentStatus.Completed ? TransactionStatus.Completed : TransactionStatus.Declined,
            };

            dbContext.PaymentTransaction.Add(transaction);
            await dbContext.SaveChangesAsync();

            PaymentProcessResponse result = new PaymentProcessResponse
            {
                Id = Id,
                MerchantOrderId = request.MerchantOrderId,
                Currency = request.Currency,
                Amount = request.Amount,
                Status = bankResult.Status == BankPaymentStatus.Completed ? TransactionStatus.Completed : TransactionStatus.Declined,
                DeclineReason = bankResult.DeclineReason.HasValue ? (TransactionDeclineReason)bankResult.DeclineReason : null,
            };

            return result;
        }
    }
}