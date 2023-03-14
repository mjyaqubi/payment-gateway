using System;
using PaymentGateway.ApplicationService.PaymentProcess.Dto;

namespace PaymentGateway.ApplicationService.PaymentProcess.Services
{
	public interface IPaymentProcessService
	{
        Task<PaymentProcessResponse> ProcessAsync(Guid merchantId, PaymentProcessRequest request);
    }
}
