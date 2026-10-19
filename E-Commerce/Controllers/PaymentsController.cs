using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace E_Commerce.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService paymentService;
        private readonly PaymentGatewaySettings stripeSettings;

        public PaymentsController(IPaymentService paymentService, IOptions<PaymentGatewaySettings> options)
        {
            this.paymentService = paymentService;
            stripeSettings = options.Value;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string basketId, CancellationToken ct)
        {
            var result = await paymentService.CreateOrUpdatePaymentIntentAsync(basketId, ct);
            return ToActionResult(result);
        }

        [HttpPost("webhook")]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(
                    json,
                    Request.Headers["Stripe-Signature"], stripeSettings.WebhookSecret);

                switch (stripeEvent.Type)
                {
                    case EventTypes.PaymentIntentSucceeded:

                        var succeededPaymentIntent = stripeEvent.Data.Object as PaymentIntent;

                        if (succeededPaymentIntent is not null)
                            await paymentService.PaymentSucceeded(succeededPaymentIntent.Id);

                        break;

                    case EventTypes.PaymentIntentPaymentFailed:

                        var failedPaymentIntent = stripeEvent.Data.Object as PaymentIntent;

                        if (failedPaymentIntent is not null)
                            await paymentService.PaymentFailed(failedPaymentIntent.Id);

                        break;

                    default:
                        break;
                }

                return Ok();
            }
            catch (StripeException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}
