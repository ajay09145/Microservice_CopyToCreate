using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;

namespace Mango.Services.CouponAPI.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/coupon")]
    [ApiController]
    //[Authorize]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private ResponseDto _responseDto;
        private IMapper _mapper;
        public CouponAPIController(AppDbContext appDbContext, IMapper mapper)
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
                IEnumerable<Coupon> coupons = _appDbContext.Coupons.ToList();
                _responseDto.Result = _mapper.Map<IEnumerable<CouponDto>>(coupons);

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
                Coupon coupon = _appDbContext.Coupons.First(x => x.CouponId == id);

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpGet]
        [Route("GetbyCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon coupon = _appDbContext.Coupons.First(x => x.CouponCode.ToLower() == code.ToLower());

                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
            }
            catch (Exception ex)
            {
                _responseDto.IsSuccess = false;
                _responseDto.Message = ex.Message;
            }
            return _responseDto;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Create([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Coupons.Add(coupon);
                _appDbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
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
        public ResponseDto Put([FromBody] CouponDto couponDto)
        {
            try
            {
                Coupon coupon = _mapper.Map<Coupon>(couponDto);
                _appDbContext.Update(coupon);
                _appDbContext.SaveChanges();
                _responseDto.Result = _mapper.Map<CouponDto>(coupon);
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
                var coupounData = _appDbContext.Coupons.First(x=>x.CouponId == id);
                if(coupounData!=null)
                {
                    _appDbContext.Remove(coupounData);
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
