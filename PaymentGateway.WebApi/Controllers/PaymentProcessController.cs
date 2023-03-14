using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PaymentGateway.ApplicationService.PaymentProcess.Dto;
using PaymentGateway.ApplicationService.PaymentProcess.Services;
using PaymentGateway.ApplicationService.PaymentHistory.Dto;
using PaymentGateway.ApplicationService.PaymentHistory.Services;
using PaymentGateway.Domain;
using PaymentGateway.Domain.Entities;


namespace PaymentGateway.WebApi.Controllers
{
    [ApiController]
    [Route("api/{merchantId:guid}/process")]
    public class PaymentProcessController : ControllerBase
    {
        private readonly IPaymentProcessService paymentProcessService;
        private readonly IPaymentHistoryService paymentHistoryService;

        public PaymentProcessController(IPaymentProcessService paymentProcessService, IPaymentHistoryService paymentHistoryService) 
        {
            this.paymentProcessService = paymentProcessService;
            this.paymentHistoryService = paymentHistoryService;
        }

        [HttpGet]
        public async Task<IActionResult> IndexAsync(Guid merchantId, [FromQuery] PaymentHistoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await paymentHistoryService.GetHistoryAsync(merchantId, request);

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessAsync(Guid merchantId, [FromBody] PaymentProcessRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await paymentProcessService.ProcessAsync(merchantId, request);

            if (result == null) {
                return StatusCode(500);
            }

            return Ok(result);
        }
    }
}
