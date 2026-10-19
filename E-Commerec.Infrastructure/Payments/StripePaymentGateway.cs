using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.Services;
using Microsoft.Extensions.Options;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.Payments
{
    public class StripePaymentGateway : IPaymentGateway
    {
        private readonly PaymentIntentService paymentIntentService = new();
        public StripePaymentGateway(IOptions<PaymentGatewaySettings> options) 
        { 
            StripeConfiguration.ApiKey = options.Value.SecretKey;
        }
        public async Task<PaymentIntentResult> CreatePaymentIntentAsync(decimal amount, string currency, CancellationToken ct = default)
        {
            var paymentIntentOptions = new PaymentIntentCreateOptions
            {
                Amount = (long)amount,
                Currency = currency.ToLower(),
                AllowedPaymentMethodTypes = ["card"],
            };

            var paymentIntent =await paymentIntentService.CreateAsync(paymentIntentOptions, cancellationToken: ct);
            return new PaymentIntentResult(paymentIntent.ClientSecret , paymentIntent.Id);
        }

        public async Task<PaymentIntentResult> UpdatePaymentIntentAsync(string paymentIntentId, decimal amount, CancellationToken ct = default)
        {
            var paymentIntentOptions = new PaymentIntentUpdateOptions
            {
                Amount = (long)amount
            };

            var paymentIntent = await paymentIntentService.UpdateAsync(paymentIntentId, paymentIntentOptions, cancellationToken: ct);
            return new PaymentIntentResult(paymentIntent.ClientSecret, paymentIntent.Id);

        }
    }

}
