using Mango.Services.ShoppingCartApi.Models.Dto;

namespace Mango.Services.ShoppingCartApi.Services.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCoupon(string couponCode);
    }
}
