using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.OrderAggregate;

namespace Talabat.Core.Specifications
{
    public class OrderWithPaymentIntentIdSpecification : BaseSpecification<Order>
    {
        public OrderWithPaymentIntentIdSpecification(string PaymentIntentId) : base(O => O.PaymentIntentId == PaymentIntentId)
        {
            
        }

    }
}
