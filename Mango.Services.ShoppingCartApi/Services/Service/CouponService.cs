using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Services.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Services.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        

        public async Task<CouponDto> GetCoupon(string couponCode)
        {

            var client = _httpClientFactory.CreateClient("CouponAPI");
            var response = await client.GetAsync($"/api/Coupon/getbycode/{couponCode}");

            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(resp.Result));

            }
            return new CouponDto();
        }
    }
}
