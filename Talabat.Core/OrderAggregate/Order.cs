﻿using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.OrderAggregate
{
    public class Order :BaseEntity <int>
    {
        public Order()
        {
            
        }
        public Order(string buyerEmail, Address shippingAddress, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subTotal, string paymentIntentId)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethod = deliveryMethod;
            Items = items;
            SubTotal = subTotal;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; }
        public DateTimeOffset OrederDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public Address ShippingAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public ICollection<OrderItem> Items { get; set;} = new HashSet<OrderItem>();
        public decimal SubTotal { get; set; }
        //derived attribute 
        //[NotMapped]
        //public decimal Total get{SubTotal + DeliveryMethod.Cost};
        //converted to method to not mapped in db 
        public decimal GetTotal() =>
            SubTotal + DeliveryMethod.Cost;

        public string PaymentIntentId { get; set; } 
    }
}