using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.OrderAggregate;

namespace Talabat.Core.Services.Contract
{
    public interface IPaymentService
    {
        Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string BasketId);
        Task<Order> UpdatePaymentIntentToSucceedOrFailed(string PaymentInetentId, bool flag);

    }
}
