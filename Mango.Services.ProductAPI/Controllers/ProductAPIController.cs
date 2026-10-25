using AutoMapper;
using Mango.Services.ProductAPI.Data;
using Mango.Services.ProductAPI.Models;
using Mango.Services.ProductAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Mango.Services.ProductAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/product")]
    [ApiController]
    //[Authorize]  //comment for show on home page without login
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDto _responseDto;
        private IMapper _mapper;
        public ProductAPIController(AppDbContext appDbContext, IMapper mapper)
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _responseDto = new ResponseDto();
        }
        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> products = _appDbContext.Products.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<ProductDto>>(products);

            }
            catch (Exception ex)
            {
                _responseDto.Result = "";
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;

            }
            return _responseDto;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product product = _appDbContext.Products.First(x => x.ProductId == id);

                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        //[HttpGet]
        //[Route("GetbyCode/{code}")]
        //public ResponseDto GetByCode(string code)
        //{
        //    try
        //    {
        //        Product product = _appDbContext.Products.First(x => x.CouponCode.ToLower() == code.ToLower());

        //        _responseDto.Result = _mapper.Map<ProductDto>(products);
        //    }
        //    catch (Exception ex)
        //    {
        //        _responseDto.IsSuccess = false;
        //        _responseDto.Message = ex.Message;
        //    }
        //    return _responseDto;
        //}

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Create([FromBody] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _appDbContext.Products.Add(product);
                _appDbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto productDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(productDto);
                _appDbContext.Update(product);
                _appDbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                var productData = _appDbContext.Products.First(x=>x.ProductId == id);
                if(productData != null)
                {
                    _appDbContext.Remove(productData);
                    _appDbContext.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }


    }
}
