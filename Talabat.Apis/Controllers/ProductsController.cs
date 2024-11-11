using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Talabat.Apis.DTOs;
using Talabat.Apis.Errors;
using Talabat.Apis.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;

namespace Talabat.Apis.Controllers
{
    public class ProductsController : ApiBaseController
    {
        public IGenericRepository<Product> _productRepository { get; set; }
        public IGenericRepository<ProductType> _productType { get; set; }
        public IGenericRepository<ProductBrand> _productBrand { get; set; }
        public IMapper _mapper;
        public ProductsController(IGenericRepository<Product> productRepository,IMapper mapper , IGenericRepository<ProductType> productType , IGenericRepository<ProductBrand> productBrand)
        {
            _productRepository = productRepository;
            _productBrand = productBrand;
            _productType = productType;
            _mapper = mapper;
        }
        [CachedAttribute(200)]
        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<IReadOnlyList<Pagination<ProductToReturnDto>>>> GetAllProducts([FromQuery]ProductSpecParams specParams) //understand it musst bind object from query not body as Get type request not has a body 
        {
            var spec = new ProductWithBrandAndTypeSpecification(specParams);
            var Products = await _productRepository.GetAllWithSpecAsync(spec);
            var MappedProducts = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductToReturnDto>>(Products);
            var CountSpec = new ProductWithFilterationForCountAsync(specParams);
            var count = await _productRepository.GetCountAsyncWithSpec(CountSpec);
            return Ok(new Pagination<ProductToReturnDto>(specParams.PageSize, specParams.PageIndex,MappedProducts , count));

        }

        [HttpGet("{id}")]
        // [ProducesResponseType(typeof(ProductToReturnDto),200 )]
        [ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound )]
        public async Task <ActionResult<Product>> GetProductById(int id )
        {
           var spec = new ProductWithBrandAndTypeSpecification(id);
            var product = await _productRepository.GetByEntityWithSpecAsync(spec);
            if (product is null) return NotFound(new ApiResponse(404));
            var FlatObj = _mapper.Map<Product, ProductToReturnDto>(product);
            if (product == null)
                return NotFound();

            return Ok(FlatObj);
        }
        [CachedAttribute(200)]
        [HttpGet("Type")]
        public async Task <ActionResult<IReadOnlyList<ProductType>>> GetTypes()
        {

            var producttypes =await _productType.GetAllAsync();
            return Ok(producttypes);
        }

        [CachedAttribute(200)]
        [HttpGet("Brand")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetBrands()
        {

            var ProductBrands = await _productBrand.GetAllAsync();
            return Ok(ProductBrands);
        }

    }
}
