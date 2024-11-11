using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text;
using Talabat.Core.Services.Contract;

namespace Talabat.Apis.Helpers
{
    public class CachedAttribute : Attribute, IAsyncActionFilter
    {
        private readonly int _expireTimeInSecond;

        public CachedAttribute(int ExpireTimeInSecond  )
        {
            _expireTimeInSecond = ExpireTimeInSecond;
            
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //inject object explicitly must allow dependcy 
           var CacheService =  context.HttpContext.RequestServices.GetRequiredService<IResponseCacheService>();
           var CacheKey = GenerateCacheKeyFromRequest(context.HttpContext.Request);
           var CachedResponse =  await CacheService.GetCachedResponse(CacheKey);
            if (!string.IsNullOrEmpty(CachedResponse))
            {
                var ContentResult = new ContentResult()
                {
                    Content = CachedResponse,
                    ContentType = "application/json",
                    StatusCode = 200,

                };
                context.Result = ContentResult;
                return;
            }

           var ExcutedContextEndPoint =  await next.Invoke();
            if(ExcutedContextEndPoint.Result is OkObjectResult result )
            {
               await CacheService.CacheResponseAsync(CacheKey , result.Value, TimeSpan.FromSeconds(_expireTimeInSecond));

            }
        }

        private string GenerateCacheKeyFromRequest(HttpRequest request)
        {
            var KeyBuilder = new StringBuilder();
            KeyBuilder.Append(request.Path); //Apis/product
            foreach(var (key ,value) in request.Query.OrderBy(x => x.Key))
            {
                KeyBuilder.Append($"|{key}-{value}");
            }
            return KeyBuilder.ToString();

        }
    }
}
