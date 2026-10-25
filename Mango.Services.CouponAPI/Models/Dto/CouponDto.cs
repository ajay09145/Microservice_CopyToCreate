namespace Mango.Services.CouponAPI.Models.Dto
{
    public class CouponDto
    {
        public int CouponId { get; set; }
        public string? CouponCode { get; set; }
        public double DicountAmount { get; set; }
        public int MinAount { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
