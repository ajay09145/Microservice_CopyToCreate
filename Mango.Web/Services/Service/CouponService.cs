
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;

namespace Mango.Web.Services.Service
{
    public class CouponService : ICouponService
    {
        private readonly IBaseService _baseService;
        public CouponService(IBaseService baseService)
        {
                _baseService = baseService;
        }


        public async Task<ResponseDto?> GetAllCouponsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.CouponAPIBase + "/api/coupon"
            });
        }        

        public async Task<ResponseDto?> GetCouponAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.CouponAPIBase + "/api/coupon/GetByCode" + couponCode
            });
        }

        public async Task<ResponseDto?> GetCouponByIdAsync(int Id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.CouponAPIBase + "/api/coupon/" + Id
            });
        }

        public async Task<ResponseDto?> DeleteCouponsAsync(int Id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.DELETE,
                Url = SD.CouponAPIBase + "/api/coupon/" + Id
            });
        }

        public async Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.POST,
                //Data = JsonConvert.SerializeObject(couponDto),
                Data = couponDto,
                Url = SD.CouponAPIBase + "/api/coupon"
            }); 
        }

        public async Task<ResponseDto?> GUpdateCouponsAsync(CouponDto couponDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.PUT,
                //Data = couponDto,
                Data = JsonConvert.SerializeObject(couponDto),
                Url = SD.CouponAPIBase + "/api/coupon"
            });
        }
    }
}
