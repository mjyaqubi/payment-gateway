
namespace PaymentGateway.ApplicationService.PaymentProcess.Dto
{
    public class BankPaymentResponse
    {
        public Guid Id { get; set; }

        public Guid PaymentGatewayOrderId { get; set; }

        public string Currency { get; set; }

        public decimal Amount { get; set; }

        public BankPaymentStatus Status { get; set; }
        public BankPaymentDeclineReason? DeclineReason { get; set; }
    }

    public enum BankPaymentStatus
    {
        Declined = 0,
        Completed = 1,
    }


    public enum BankPaymentDeclineReason
    {
        Refused = 1, // The transaction was refused.
        Referral = 2, // Referrals
        Acquirer_Error = 3, // The transaction did not go through due to an error that occurred on the acquirer's end.
        Blocked_Card = 4, // The card used for the transaction is blocked, therefore unusable.
        Expired_Card = 5, // The card used for the transaction has expired. Therefore it is unusable.
        Invalid_Amount = 6, // An amount mismatch occurred during the transaction process.
        Invalid_Card_Number = 7, // The specified card number is incorrect or invalid.
        Issuer_Unavailable = 8, // It is not possible to contact the shopper's bank to authorise the transaction.
        Not_supported = 9, // The shopper's bank does not support or does not allow this type of transaction.
        The_3D_Not_Authenticated = 10, // 3D Secure authentication was not executed, or it did not execute successfully.
        Not_enough_balance = 11, // The card does not have enough money to cover the payable amount.
        Acquirer_Fraud = 12, // Possible fraud.
        Cancelled = 13, // The transaction was cancelled.
        Shopper_Cancelled = 14, // The shopper cancelled the transaction before completing it.
        Invalid_Pin = 15, // The specified PIN is incorrect or invalid.
        Pin_tries_exceeded = 16, // The shopper specified an incorrect PIN more that three times in a row.
        Pin_validation_not_possible = 17, // It is not possible to validate the specified PIN number.
        FRAUD = 18, // The pre-authorisation risk checks resulted in a fraud score of 100 or more. Therefore, the transaction was flagged as fraudulent, and was refused.
        Not_Submitted = 19, // The transaction was not submitted correctly for processing.
        FRAUD_CANCELLED = 20, // The sum of pre-authorisation and post-authorisation risk checks resulted in a fraud score of 100 or more. Therefore, the transaction was flagged as fraudulent, and was refused.
        Transaction_Not_Permitted = 21, // Transaction not permitted to issuer/cardholder 
        CVC_Declined = 22, // The specified CVC (card security code) is invalid.
        Restricted_Card = 23, // Restricted Card or Invalid card in this country
        Revocation_Of_Auth = 24, // Indicates that the shopper requested to stop a subscription.
        Declined_Non_Generic = 25, // This response maps all those response codes that cannot be reliably mapped. This makes it easier to tell generic declines (for example, Mastercard "05: Do not honor" response) from more specific ones.
        Withdrawal_amount_exceeded = 26, // The withdrawal amount permitted for the shopper's card has exceeded.
        Withdrawal_count_exceeded = 27, // The number of withdrawals permitted for the shopper's card has exceeded.
        Issuer_Suspected_Fraud = 28, // Issuer reported the transaction as suspected fraud.
        AVS_Declined = 29, // The address data the shopper entered is incorrect.
        Card_requires_online_pin = 30, // The shopper's bank requires the shopper to enter an online PIN.
        No_checking_account_available_on_Card = 31, // The shopper's bank requires a checking account to complete the purchase.
        No_savings_account_available_on_Card = 32, // The shopper's bank requires a savings account to complete the purchase.
        Mobile_pin_required = 33, // The shopper's bank requires the shopper to enter a mobile PIN.
        Contactless_fallback = 34, // The shopper abandoned the transaction after they attempted a contactless payment and were prompted to try a different card entry method (PIN or swipe).
        Authentication_required = 35, // The issuer declined the authentication exemption request and requires authentication for the transaction. Retry with 3D Secure.
        RReq_not_received_from_DS = 36, // The issuer or the scheme wasn't able to communicate the outcome via RReq.
        Current_AID_is_in_Penalty_Box = 37, // The payment network can't be reached. Retry the transaction with a different payment method.
        CVM_Required_Restart_Payment = 38, // A PIN or signature is required. Retry the transaction.
        The_3DS_Authentication_Error = 39 // The 3D Secure authentication failed due to an issue at the card network or issuer. Retry the transaction, or retry the transaction with a different payment method.
    }
}
