using PaymentGateway.Domain.Entities;

namespace PaymentGateway.ApplicationService.PaymentProcess.Dto
{
    public class PaymentProcessResponse
    {
        public Guid Id { get; set; }

        public Guid MerchantOrderId { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public TransactionStatus Status { get; set; }

        public TransactionDeclineReason? DeclineReason { get; set; }
    }
}
