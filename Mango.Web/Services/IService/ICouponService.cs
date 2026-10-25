
using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface ICouponService
    {
        Task<ResponseDto?> GetCouponAsync(string couponCode);
        Task<ResponseDto?> GetAllCouponsAsync();
        Task<ResponseDto?> GetCouponByIdAsync(int Id);
        Task<ResponseDto?> CreateCouponsAsync(CouponDto couponDto);
        Task<ResponseDto?> GUpdateCouponsAsync(CouponDto couponDto);
        Task<ResponseDto?> DeleteCouponsAsync(int Id);

       
    }
}
