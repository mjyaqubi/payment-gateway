using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PaymentGateway.ApplicationService.Models;
using PaymentGateway.ApplicationService.PaymentHistory.Dto;
using PaymentGateway.ApplicationService.PaymentProcess.Dto;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.ApplicationService.PaymentHistory.Services
{
	public class PaymentHistoryService: IPaymentHistoryService
	{
        private readonly PaymentGatewayDbContext dbContext;

        public PaymentHistoryService(PaymentGatewayDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<PaymentTransaction>> GetHistoryAsync(Guid merchantId, PaymentHistoryRequest request)
        {
            var query = dbContext.PaymentTransaction.Where(x => x.MerchantId == merchantId);

            if (request.MerchantOrderId.HasValue) {
                query = query.Where(x => x.MerchantOrderId == request.MerchantOrderId);
            }

            if (!string.IsNullOrWhiteSpace(request.CardNumber)) {
                query = query.Where(x => x.CardNumber == request.CardNumber);
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= request.StartDate);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= request.EndDate);
            }

            return await query.ToListAsync();
        }
    }
}

