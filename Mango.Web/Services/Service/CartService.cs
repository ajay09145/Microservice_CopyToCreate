
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mango.Web.Services.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService _baseService;
        public CartService(IBaseService baseService)
        {
                _baseService = baseService;
        }

        public async Task<ResponseDto?> GetCartByUserIdAsync(string userId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.ShoppingCartAPIBase + "/api/cart/getcart/" + userId
            });
        }

        public async Task<ResponseDto?> UpsertCartAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.POST,
                Data = cartDto,
                //Data = JsonConvert.SerializeObject(cartDto),
                Url = SD.ShoppingCartAPIBase + "/api/cart/cartupsert"
            });
        }

        public async Task<ResponseDto?> RemoveFromCartAsync(int cartDetailsId)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.POST,
                Data = cartDetailsId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/removecart"
            });
        }

        public async Task<ResponseDto?> ApplyCouponAsync(CartDto cartDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.POST,
                Data = cartDto,
                //Data = JsonConvert.SerializeObject(cartDto),
                Url = SD.ShoppingCartAPIBase + "/api/cart/applycart"
            });
        }
    }
}
