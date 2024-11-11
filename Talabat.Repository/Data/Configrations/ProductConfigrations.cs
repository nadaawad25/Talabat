using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Repository.Data.Configrations
{
    public class ProductConfigrations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            
            builder.Property(P => P.Name).HasMaxLength(200).IsRequired();
            builder.Property(P => P.PictureUrl).IsRequired(true);
            builder.Property(P => P.Price).HasColumnType("decimal(18,2)").IsRequired();

            builder.HasOne(P => P.ProductBrand)
                .WithMany()
                .HasForeignKey(P => P.ProductBrandId);

            builder.HasOne(P => P.ProductType)
                .WithMany()
                .HasForeignKey(P => P.ProductTypeId);
          
            builder.Property(P => P.ProductBrandId).IsRequired(true);
            builder.Property(P => P.ProductTypeId).IsRequired(true);


        }
    }
}
