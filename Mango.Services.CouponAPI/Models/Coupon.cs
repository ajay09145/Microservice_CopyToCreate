using System.ComponentModel.DataAnnotations;

namespace Mango.Services.CouponAPI.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string? CouponCode { get; set; }
        [Required]
        public double DicountAmount { get; set; }
        public int MinAount { get; set; }
        public DateTime LastUpdated { get; set; }
       // public int CouponId { get; set; }
    }
}
