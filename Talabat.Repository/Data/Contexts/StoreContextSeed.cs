using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.OrderAggregate;

namespace Talabat.Repository.Data.Contexts
{
    public static class StoreContextSeed
    {
        public static async Task SeedAsync(StoreDbContext context)
        {
            if (!context.ProductBrands.Any())
            {
                var BrandsData = File.ReadAllText("../Talabat.Repository/DataSeed/brands.json");
                var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(BrandsData);
                if (Brands?.Count() > 0)
                {
                    foreach (var item in Brands)
                        await context.Set<ProductBrand>().AddAsync(item);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.ProductTypes.Any())
            {
                var TypesData = File.ReadAllText("../Talabat.Repository/DataSeed/types.json");
                var Types = JsonSerializer.Deserialize<List<ProductType>>(TypesData);
                if (Types?.Count() > 0)
                {
                    foreach (var type in Types)
                        await context.Set<ProductType>().AddAsync(type);
                    await context.SaveChangesAsync();
                }
            }

            if (!context.Products.Any())
            {
                var ProductsData = File.ReadAllText("../Talabat.Repository/DataSeed/products.json");
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductsData);
                if (Products?.Count() > 0)
                {
                    foreach (var product in Products)
                        await context.Set<Product>().AddAsync(product);
                    await context.SaveChangesAsync();
                }
            }


            if (!context.DeliveryMethods.Any())
            {
                var DeliveryMethodsData = File.ReadAllText("../Talabat.Repository/DataSeed/delivery.json");
                var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(DeliveryMethodsData);
                if (DeliveryMethods?.Count() > 0)
                {
                    foreach (var DeliveryMethod in DeliveryMethods)
                        await context.Set<DeliveryMethod>().AddAsync(DeliveryMethod);
                    await context.SaveChangesAsync();
                }
            }


        }

       
    }
}
