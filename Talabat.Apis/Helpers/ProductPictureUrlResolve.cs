using AutoMapper;
using Talabat.Apis.DTOs;
using Talabat.Core.Entities;

namespace Talabat.Apis.Helpers
{
    public class ProductPictureUrlResolve : IValueResolver<Product, ProductToReturnDto, string>
    {
        private IConfiguration _configuration;
        public ProductPictureUrlResolve(IConfiguration configuration)
        {
            _configuration = configuration;   
        }
        public string Resolve(Product source, ProductToReturnDto destination, string destMember, ResolutionContext context)
        {
         if(!String.IsNullOrEmpty(source.PictureUrl))
            {
                return $"{_configuration["ApiBaseUrl"]}{source.PictureUrl}";
            }
            return string.Empty;
                
        }
    }
}
