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

namespace Talabat.Service.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService (IBasketRepository basketRepository , IUnitOfWork unitOfWork , IPaymentService paymentService) {
        
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _paymentService = paymentService;
        }
        public async Task<Order?> CreateOrderAsync(string BuyerEmail, string BasketId, int DeliveryMethod, Address ShippingAddress)
        { 
           var Basket =  await _basketRepository.GetBasketAsync(BasketId);
            var OrderItems = new List<OrderItem>();
            if(Basket?.Items.Count > 0)
            {
                foreach (var  item in Basket.Items)
                {
                    var product =await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);

                    if (product is null)
                    {
                        Console.WriteLine($"Product with ID {item.Id} not found.");
                        continue;  
                    }
                    var productItemOrdered = new ProductItemOrdered(product.Id , product.Name , product.PictureUrl);
                    var OrderItem = new OrderItem(productItemOrdered, item.Quantity, item.Price);
                    OrderItems.Add(OrderItem);

                }
            }
            var SubTotal = OrderItems.Sum(item => item.Price * item.Quantity);

            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(DeliveryMethod);
            var Spec = new OrderWithPaymentIntentIdSpecification(Basket.PaymentId);
            var ExOrder = await _unitOfWork.Repository<Order>().GetByEntityWithSpecAsync(Spec);
           
            if(ExOrder is not null)
            {
                _unitOfWork.Repository<Order>().Delete(ExOrder);
               await _paymentService.CreateOrUpdatePaymentIntent(BasketId);
            }

            var order = new Order(BuyerEmail , ShippingAddress , deliveryMethod, OrderItems, SubTotal , Basket.PaymentId);
            await _unitOfWork.Repository<Order>().AddAsync(order);  
            var result =  await _unitOfWork.CompleteAsync();
            if (result <= 0)
                return null;
            return order;
        }

        public async Task<Order?> GetOrderById(string BuyerEmail, int OrderId)
        {
            var spec = new OrderSpecification(BuyerEmail , OrderId);
            var order = await _unitOfWork.Repository<Order>().GetByEntityWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string BuyerEmail)
        {
            var spec = new OrderSpecification(BuyerEmail);
            var Orders =await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);
            return Orders;
        }
    }
}



