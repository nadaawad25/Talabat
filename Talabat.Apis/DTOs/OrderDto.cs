using System.ComponentModel.DataAnnotations;
using Talabat.Core.OrderAggregate;

namespace Talabat.Apis.DTOs
{
    public class OrderDto
    {
        [Required]
        public string  BasketId { get; set; }
        [Required]
        public int  DeliveryMethodId { get; set; }
        [Required]
        public AddressDto  ShipToAddress { get; set; }
    }
}
