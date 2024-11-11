using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Talabat.Apis.Errors;
using Talabat.Apis.Helpers;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Repository.Data.Contexts;
using Talabat.Repository.Repositories;
using Talabat.Service.Services;

namespace Talabat.Apis.Extentions
{
    public  static class AppServices
    {
       public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IBasketRepository), typeof(BasketRepository));
            //services.AddAutoMapper(M => M.AddProfile(new MappingProfiles()));
            services.AddAutoMapper(typeof(MappingProfiles));
            //ApiBehiorOptions is a class
            services.Configure<ApiBehaviorOptions>(options =>
            {//Properity in class
                options.InvalidModelStateResponseFactory = (ActionContext) =>
                {
                    var errors = ActionContext.ModelState.Where(d => d.Value.Errors.Count > 0)
                                 .SelectMany(d => d.Value.Errors).Select(e => e.ErrorMessage).ToList();
                    var ValidationErrorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErrorResponse);
                };
            });

            services.AddScoped<IUnitOfWork , UnitOfWork>();
            services.AddScoped<IOrderService , OrderService>();
            services.AddScoped<IPaymentService , PaymentService>();
            
            return services;
        }

    }
}
