using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(IConfiguration configuration ,IBasketRepository basketRepository ,IUnitOfWork unitOfWork )
        {
            _configuration = configuration;
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
        }
        //amount of basket and order , create paymentintent 
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:Secretkey"];
           var Basket = await _basketRepository.GetBasketAsync( BasketId );
            if (Basket is null)
                return null;
            var ShippingPrice = 0M;
            if (Basket.DeliveryMethodId.HasValue)
            {
              var DelivaryMethod =  await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(Basket.DeliveryMethodId.Value);
               ShippingPrice = DelivaryMethod.Cost;
            }

            if( Basket.Items.Count > 0)
            {
                foreach ( var item in Basket.Items )
                {
                    var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                    if(item.Price != product.Price)
                        item.Price = product.Price;
                }
                var SubTotal = Basket.Items.Sum(p => p.Price * p.Quantity);
                var service = new PaymentIntentService();
                PaymentIntent paymentIntent;
                if(string.IsNullOrEmpty(Basket.PaymentId) ) 
                { 
                    var options = new PaymentIntentCreateOptions()
                    {
                        Amount = (long)ShippingPrice * 100 + (long)SubTotal * 100,
                        Currency = "usd",
                        PaymentMethodTypes = new List<string>() { "card" }
                    };
                    paymentIntent = await service.CreateAsync(options);
                    Basket.PaymentId = paymentIntent.Id;
                    Basket.ClientSecret = paymentIntent.ClientSecret;

                }
                else
                {
                    var options = new PaymentIntentUpdateOptions()
                    {
                        Amount = (long)ShippingPrice * 100 + (long)SubTotal * 100,

                    };
                    paymentIntent=  await service.UpdateAsync(Basket.Id , options);
                    Basket.PaymentId = paymentIntent.Id;
                    Basket.ClientSecret= paymentIntent.ClientSecret;
                }
            }
            await _basketRepository.UpdateBasketAsync(Basket);
            return Basket;
        }

        public async Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentInetentId, bool flag)
        {
            var Spec = new OrderWithPaymentIntentIdSpecification(PaymentInetentId);
            var Order = await _unitOfWork.Repository<Order>().GetByEntityWithSpecAsync(Spec);
            if(flag )
            {
                Order.Status = OrderStatus.PaymentRecived;
            }
            else
            {
                Order.Status = OrderStatus.PaymentFailed;
            }
            _unitOfWork.Repository<Order>().Update(Order);
            await _unitOfWork.CompleteAsync();
            return Order;

        }
    }
}
