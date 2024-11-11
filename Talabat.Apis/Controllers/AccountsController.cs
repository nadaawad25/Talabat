using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.Apis.Extentions;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Services.Contract;

namespace Talabat.Apis.Controllers
{
    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        public AccountsController(UserManager<AppUser> userManager ,
             SignInManager<AppUser> signInManager,
             ITokenService tokenService,
             IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto user)
        {
            if (CheckEmailExist(user.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "Email is Already Exists"));
            var User = new AppUser()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                UserName = user.Email.Split('@')[0]

            };
           var Result =  await _userManager.CreateAsync(User ,user.Password);
            if (Result.Succeeded)
            {
                var ReturnedUser = new UserDto()
                {
                    DisplayName = user.DisplayName,
                    Email = user.Email,
                    Token =await _tokenService.CreateTokenAsync(User, _userManager)

                };
                return Ok(ReturnedUser);
            }else
            return BadRequest(new ApiResponse(400));
           
        }
        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var Result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!Result.Succeeded)
                return BadRequest();
            return Ok(new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = model.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            });
        }
        //Static Segment
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
           var Email =  User.FindFirstValue(ClaimTypes.Email);//Prop in ControllerBase has Claims of user => Email 
           var user = await _userManager.FindByEmailAsync(Email);
            var ReturnedObject = new UserDto()
            {
                DisplayName = user.DisplayName,
                Email = user.Email,
                Token = await _tokenService.CreateTokenAsync(user, _userManager)
            };
            return Ok(ReturnedObject);
        }

        [HttpGet("UserAddress")]
        [Authorize]
        public async Task<ActionResult<AddressDto>> GetCurrentUserAddress()
        {
            //var Email = User.FindFirstValue(ClaimTypes.Email);
            //var user = await _userManager.FindByEmailAsync(Email); USe Extention Method
            var user = await _userManager.FindUserWithAddreesAsync(User);
            var AddressDto = _mapper.Map<Address , AddressDto>(user.Address);
            return Ok(AddressDto);

        }
        [Authorize]
        [HttpPut("UserAddress")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(AddressDto updatedAddress)
        {
            //var email = User.FindFirstValue(ClaimTypes.Email);
            //var user = await _userManager.FindByEmailAsync(email);
            //var MappedAddress = _mapper.Map<AddressDto, Address>(updatedAddress);
            var user = await _userManager.FindUserWithAddreesAsync(User);
            var mappedAddress = _mapper.Map<AddressDto, Address>(updatedAddress);
            mappedAddress.Id = user.Address.Id;
            user.Address = mappedAddress;
            var Result = await _userManager.UpdateAsync(user);
            if (!Result.Succeeded)
                return BadRequest(new ApiResponse(400));
            return Ok(updatedAddress);
        }
        [Authorize]
        [HttpGet("ExistsEmail")]

        public async Task<ActionResult<bool>> CheckEmailExist(string Email)
        {
             return await  _userManager.FindByEmailAsync(Email) is not null;
        }

       

       
    }

}
