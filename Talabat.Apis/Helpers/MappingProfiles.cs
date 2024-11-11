using AutoMapper;
using Talabat.Apis.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.OrderAggregate;

namespace Talabat.Apis.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(D => D.ProductType ,options => options.MapFrom(S => S.ProductType.Name))
                .ForMember(D => D.ProductBrand , options => options.MapFrom(S => S.ProductBrand.Name))
                .ForMember(D => D.PictureUrl ,options => options.MapFrom<ProductPictureUrlResolve>());

            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
            CreateMap<CustomerBasket,CustomerBasketDto>().ReverseMap();
            CreateMap<BasketItem, BasketItemDto>().ReverseMap();
            CreateMap<AddressDto , Core.OrderAggregate.Address>().ReverseMap();
            CreateMap<Order, OrderToReturnDto>()
                .ForMember(d => d.DeliveryMethod, options => options.MapFrom(S => S.DeliveryMethod.ShortName))
                .ForMember(d => d.DeliveryMethodCost, options => options.MapFrom(S => S.DeliveryMethod.Cost));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, options => options.MapFrom(S => S.Product.ProductId))
                .ForMember(d => d.ProductName, options => options.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, options => options.MapFrom(s => s.Product.PictureUrl))
                .ForMember(d => d.PictureUrl, options => options.MapFrom<OrderItemPictureUrlResolver>());
        }
    }
}
