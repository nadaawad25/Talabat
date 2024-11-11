using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.OrderAggregate;

namespace Talabat.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<Order?> CreateOrderAsync(string BuyerEmail , string BasketId ,int DeliveryMethod, Address ShippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForSpecificUserAsync(string BuyerEmail);
        Task<Order?> GetOrderById(string BuyerEmail , int OrderId);
    }
}
