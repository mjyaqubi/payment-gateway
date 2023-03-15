using System;
using System.Diagnostics;
using System.Net;
using AcquiringBank.Api.Controllers;
using AcquiringBank.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AcquiringBank.Tests.Controllers
{
    [TestClass]
    public class PaymentProcessControllerTests
    {
        private readonly PaymentProcessController controller;

        public PaymentProcessControllerTests()
        {
            controller = new PaymentProcessController();
        }

        [TestMethod]
        public void POST_Process_Should_Succeed()
        {
            // Arrange
            var paymentGatewayId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            var requestBody = new BankPaymentRequest
            {
                PaymentGatewayOrderId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa7"),
                CardNumber = "5390199394005593",
                ExpiryMonth = "01",
                ExpiryYear = "25",
                CVV = "123",
                Currency = "SEK",
                Amount = 1000,
            };

            // Act
            var result = controller.Process(paymentGatewayId, requestBody) as OkObjectResult;
            var data = result.Value as BankPaymentResponse;

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            Assert.IsNotNull(data.Id);
            Assert.IsNotNull(data.PaymentGatewayOrderId);
            Assert.AreEqual(data.Currency, "SEK");
            Assert.AreEqual(data.Amount, 1000);
            Assert.IsNotNull(data.Status);
            Assert.IsTrue(Enum.IsDefined(typeof(BankPaymentStatus), data.Status));
            if (data.Status == BankPaymentStatus.Declined)
            {
                Assert.IsNotNull(data.DeclineReason);
                Assert.IsTrue(Enum.IsDefined(typeof(BankPaymentDeclineReason), data.DeclineReason));
            }
        }

        [TestMethod]
        public void POST_Process_Should_Throws_Validation_Error()
        {
            // Arrange
            var paymentGatewayId = Guid.Parse("3fa85f64-5717-4562-b3fc-2c963f66afa6");
            controller.ModelState.AddModelError("error", "some error");

            // Act
            var result = controller.Process(paymentGatewayId, new BankPaymentRequest { });


            // Assert
            Assert.IsTrue(result is BadRequestObjectResult);
        }
    }
}