using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.V2;
using Stripe.V2.Core;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.Core.Services.Contract;
using Talabat.Service.Services;

namespace Talabat.Apis.Controllers
{
    public class PaymentsController : ApiBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IConfiguration _configuration;

        public PaymentsController(IPaymentService paymentService , IConfiguration configuration)
        {
            _paymentService = paymentService;
            _configuration = configuration;
        }
        [ProducesResponseType(typeof(CustomerBasketDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(CustomerBasketDto) , StatusCodes.Status400BadRequest)]
        [HttpPost("{BasketId}")]
        public async Task<ActionResult<CustomerBasketDto>> CreateOrUpdatePaymentIntent(string BasketId)
        {
            var Basket = await _paymentService.CreateOrUpdatePaymentIntent(BasketId);
            if (Basket is null)
                return BadRequest(new ApiResponse(400 , "There is a Problem with your Basket "));
            return Ok(Basket);
        }


        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeSignature = Request.Headers["Stripe-Signature"];
            if (string.IsNullOrEmpty(stripeSignature))
            {
                return BadRequest(new ApiResponse(400, "Missing Stripe signature"));
            }

            var stripeEvent = Stripe.EventUtility.ConstructEvent(json, stripeSignature, _configuration["StripeSettings:WebhookSecret"]);

            if (stripeEvent.Type == "payment_intent.succeeded")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    // Handle successful payment
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, true);
                }
                else
                {
                    // Handle null payment intent
                    return BadRequest(new ApiResponse(400, "PaymentIntent object is null"));
                }
            }
            else if (stripeEvent.Type == "payment_intent.failed")
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent != null)
                {
                    // Handle failed payment
                    await _paymentService.UpdatePaymentIntentToSucceedOrFailed(paymentIntent.Id, false);
                }
                else
                {
                    // Handle null payment intent
                    return BadRequest(new ApiResponse(400, "PaymentIntent object is null"));
                }
            }
            return Ok();
        }





    }
}
