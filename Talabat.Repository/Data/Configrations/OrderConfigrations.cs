﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.OrderAggregate;

namespace Talabat.Repository.Data.Configrations
{
    public class OrderConfigrations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Status)
                 .HasConversion(OStatus => OStatus.ToString(),OStatus => (OrderStatus) Enum.Parse(typeof(OrderStatus), OStatus));
            builder.Property(O => O.SubTotal)
                .HasColumnType("decimal(18,2)");
            builder.OwnsOne(O => O.ShippingAddress, S => S.WithOwner());
            builder.HasOne(o => o.DeliveryMethod)
                .WithMany()
                .OnDelete(DeleteBehavior.NoAction);
                

        }
    }
}
