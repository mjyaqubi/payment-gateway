using AcquiringBank.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace AcquiringBank.Api.Controllers
{
    [ApiController]
    [Route("api/{paymentGatewayId:guid}/process")]
    public class PaymentProcessController : ControllerBase
    {
        [HttpPost()]
        public IActionResult Process(Guid paymentGatewayId, [FromBody] BankPaymentRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var status = new Random().Next(100) % 2 == 0 ? BankPaymentStatus.Declined : BankPaymentStatus.Completed;
            var result = new BankPaymentResponse
            {
                Id = Guid.NewGuid(),
                Amount = request.Amount,
                Currency = request.Currency,
                Status = status,
                PaymentGatewayOrderId = request.PaymentGatewayOrderId,
            };

            if (status == BankPaymentStatus.Declined)
            {
                Array values = Enum.GetValues(typeof(BankPaymentDeclineReason));
                Random random = new Random();
                result.DeclineReason = (BankPaymentDeclineReason)values.GetValue(random.Next(values.Length));
            }

            return Ok(result);
        }
    }
}
