using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.Core.Entities;
using Talabat.Core.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;

namespace Talabat.Apis.Controllers
{
    public class OrdersController : ApiBaseController
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketRepository _basketRepository;

        public OrdersController(IOrderService orderService , IMapper mapper , IUnitOfWork unitOfWork , IBasketRepository basketRepository) 
        {
            _orderService = orderService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _basketRepository = basketRepository;
        }
        //create oreder (frontend send you => basket is , shipping address s, delivery method id )
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(Order), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse) , StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Order>> CreateOrder(OrderDto orderDto)
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var MappedAddress = _mapper.Map<AddressDto, Address>(orderDto.ShipToAddress);

            var basket = await _basketRepository.GetBasketAsync(orderDto.BasketId);
            if (basket is null)
            {
                return BadRequest(new ApiResponse(400, "Basket not found"));
            }
            var originalBasket = _mapper.Map<CustomerBasket>(basket);

            basket.DeliveryMethodId = orderDto.DeliveryMethodId;
            var updatedBasket = await _basketRepository.UpdateBasketAsync(basket);

            var order = await _orderService.CreateOrderAsync(BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, MappedAddress);

            if (order is null)
            {
                await _basketRepository.UpdateBasketAsync(originalBasket);

                return BadRequest(new ApiResponse(400, "There is a problem with your order"));
            }

            return Ok(order);
        }


        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]

        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Orders = await _orderService.GetOrdersForSpecificUserAsync(BuyerEmail);
            if (Orders is null)
                return NotFound(new ApiResponse(404, "There is No Orders for this User"));
            var MappedOrders = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(Orders);

            return Ok(MappedOrders);
        }


        [Authorize]
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Order>> GetOrderForSpacificUser(int id )
        {
            var BuyerEmail = User.FindFirstValue(ClaimTypes.Email);
            var Order = await _orderService.GetOrderById(BuyerEmail ,id  );
            if (Order is null)
                return NotFound(new ApiResponse(404, $"There is  Order with {id} for this User"));
            var MappedOrder = _mapper.Map<Order, OrderToReturnDto>(Order);
            return Ok(MappedOrder);
        }


       
        [HttpGet("DeliveryMethod")]
        public async Task<ActionResult<DeliveryMethod>> GetDeliveryMethod()
        {
          var deliveryMethod =   await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();   
          return Ok(deliveryMethod);
        }

    }
}
