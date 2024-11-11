using System.Reflection.Metadata.Ecma335;

namespace Talabat.Apis.Errors
{
    public class ApiResponse 
    {
        public int? StatusCode { get; set; }

        public string? Message { get; set; }

        public ApiResponse(int statusCode , string? message = null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode);

            
        }
        public string? GetDefaultMessageForStatusCode(int statusCode)
        {
            //C# 7 => Switch Expression 
            return StatusCode switch
            {
                400 => "Bad Request",
                401 => "You Are Not Authorized",
                404 => "Resource Not Found ",
                500 => "Internal Server Error",
                _ => null
            };
        }

        //500 => Internal Server Error 
        //400 => Bad Request
        //401 => UnAuthorized
        //404 => NotFound
        
    }
}
