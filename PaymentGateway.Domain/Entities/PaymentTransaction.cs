namespace PaymentGateway.Domain.Entities
{
    public class PaymentTransaction
    {
        public Guid Id { get; set; }
        public Guid MerchantId { get; set; }
        public Guid MerchantOrderId { get; set; }
        public Guid BankPaymentId { get; set; }
        public string CardNumber { get; set; }
        public string MaskedCardNumber { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
        public TransactionStatus Status { get; set; }
        public TransactionDeclineReason? DeclineReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}