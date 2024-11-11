using Microsoft.AspNetCore.Mvc;
using Talabat.Apis.Errors;
using Talabat.Repository.Data.Contexts;

namespace Talabat.Apis.Controllers
{
    public class BuggyController : ApiBaseController
    {
        private StoreDbContext _dbContext;
        public BuggyController(StoreDbContext storeDbContext) {
            _dbContext = storeDbContext;
        }
        [HttpGet("NotFound")]
        public ActionResult GetNotFoundRequest()
        {
           var product =  _dbContext.Products.Find(100);
            if(product is null )
                return NotFound(new ApiResponse (404));
            return Ok(product);
        }
        [HttpGet("ServerError")]
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);
            var productreturn = product.ToString();
            //Will throw Exception Null Refrence Expception 
            return Ok(productreturn);

        }
        [HttpGet("BadRequest")]
        public ActionResult GetBadRequest()
        {
            return BadRequest();
        }

        [HttpGet("BadRequest/{id}")]
        public ActionResult GetBadRequest(int id)
        {
            return Ok();
        }

    }
}
