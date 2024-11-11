using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Apis.Controllers
{
    public class BasketController : ApiBaseController
    {

        private IBasketRepository _basketRepository;
        private IMapper _mapper;
        public BasketController(IBasketRepository basketRepository, IMapper mapper)
        {
            _basketRepository = basketRepository;
            _mapper = mapper;
        }
        //Get Or ReCreate 
        // [HttpGet("{BasketId}")] not need to tell Clr enter in endpoint as it is a query not object 
        [HttpGet("{BasketId}")]
        public async Task<ActionResult<CustomerBasket>> GetCustomerBasket (string BasketId)
        {
            var Basket = await _basketRepository.GetBasketAsync(BasketId);
            return Basket is null ? new CustomerBasket(BasketId) : Ok(Basket); 
        }
        //Create or Update

        [HttpPost]
        public async Task<ActionResult<CustomerBasket>> UpdateBasket(CustomerBasketDto basket)
        {
            var MappedBasket = _mapper.Map<CustomerBasketDto, CustomerBasket>(basket);
            var CreatedOrUpdatedBasket = await _basketRepository.UpdateBasketAsync(MappedBasket);
            if (CreatedOrUpdatedBasket is null)
                return BadRequest(new ApiResponse(400));
            return Ok(CreatedOrUpdatedBasket);
        }
        [HttpDelete("{BasketId}")]
        public async Task<ActionResult<bool>> DeleteBasket (string BasketId)
        {
            return await _basketRepository.DeleteBasketAsync(BasketId);
        }

    }
}
