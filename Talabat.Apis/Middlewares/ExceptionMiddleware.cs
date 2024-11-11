using System.Net;
using System.Text.Json;
using Talabat.Apis.Errors;

namespace Talabat.Apis.Middlewares
{
    public class ExceptionMiddleware
    {
        private RequestDelegate _next;
        private ILogger<ExceptionMiddleware> _logger; 
        private IHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next , ILogger<ExceptionMiddleware> logger ,IHostEnvironment hostEnvironment )
        {
            _next = next;
            _logger = logger;
            _environment = hostEnvironment;
        }
        public async Task InvokeAsync(HttpContext httpContext)

        {
            try
            {
               await _next.Invoke(httpContext);
            }catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                //Production Log => in BD
                //in Development 
                httpContext.Response.ContentType = "application/json";
                // httpContext.Response.StatusCode = 500;

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                //if( _environment.IsDevelopment() )
                //{
                //    var response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError , e.Message, e.StackTrace.ToString());
                //}
                //else
                //{
                //    var response = new ApiExceptionResponse((int)HttpStatusCode.InternalServerError);
                //}

                var response = _environment.IsDevelopment() ? new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, e.Message, e.StackTrace.ToString()) : new ApiExceptionResponse((int)HttpStatusCode.InternalServerError, e.Message, e.StackTrace.ToString());

                var options = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var jsonResponse = JsonSerializer.Serialize(response , options);
                await httpContext.Response.WriteAsync(jsonResponse);
            }
        }
    }
}
