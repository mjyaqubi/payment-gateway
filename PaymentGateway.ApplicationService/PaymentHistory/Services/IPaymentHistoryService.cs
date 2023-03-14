using System;
using PaymentGateway.ApplicationService.PaymentHistory.Dto;
using PaymentGateway.Domain.Entities;

namespace PaymentGateway.ApplicationService.PaymentHistory.Services
{
	public interface IPaymentHistoryService
	{
        Task<IEnumerable<PaymentTransaction>> GetHistoryAsync(Guid merchantId, PaymentHistoryRequest request);
    }
}
