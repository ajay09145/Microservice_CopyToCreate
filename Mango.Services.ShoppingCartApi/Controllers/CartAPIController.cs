using AutoMapper;
using Mango.Services.ShoppingCartApi.Data;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Services.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;

namespace Mango.Services.ShoppingCartApi.Controllers
{
    [Route("api/cart")]
    [ApiController]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private readonly IProductService _productService;
        private readonly ICouponService _couponService;
        public CartAPIController(IMapper mapper, AppDbContext db, IProductService productService, ICouponService couponService)
        {
            _db = db;
            _mapper = mapper;
            this._response = new ResponseDto();
            _productService = productService;
            _couponService = couponService;
        }

        [HttpGet("getcart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cartDto = new CartDto()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == userId))
                };               
                cartDto.CartDetails = _mapper.Map<IEnumerable<CartDetailsDto>>(await _db.CartDetails
                    .Where(u=>u.CartHeaderId == cartDto.CartHeader.CartHeaderId).ToListAsync());

                IEnumerable<ProductDto> productList = await _productService.GetAllProducts();

                foreach (var item in cartDto.CartDetails)
                {
                    item.Product = productList.FirstOrDefault(u => u.ProductId == item.ProductId);
                    cartDto.CartHeader.CartTotal += (item.Count * item.Product.Price);
                }

                if(!string.IsNullOrEmpty(cartDto.CartHeader.CouponCode))
                {
                    CouponDto coupon = await _couponService.GetCoupon(cartDto.CartHeader.CouponCode);
                    if(coupon != null && cartDto.CartHeader.CartTotal > coupon.MinAount)
                    {
                        cartDto.CartHeader.CartTotal -= coupon.DicountAmount;
                        cartDto.CartHeader.Discount = coupon.DicountAmount;
                        
                    }
                }
                else
                {
                    cartDto.CartHeader.Discount = 0;
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("cartupsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //cretae the header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    await _db.CartHeaders.AddAsync(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId && 
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if(cartDetailsFromDb == null)
                    {
                        //Created  cart details
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        await _db.CartDetails.AddAsync(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                    else
                    {
                        //update count cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();

                    }
                }
                _response.Result = cartDto; 
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("removecart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = await _db.CartDetails.FirstAsync(u => u.CartDetailsId == cartDetailsId);
                int totalCountItems = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountItems == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders.FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);
                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }

        [HttpPost("applycart")]
        public async Task<ResponseDto> ApplyCart([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


        [HttpPost("removecart")]
        public async Task<ResponseDto> RemoveCart([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = "";
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.Message = ex.Message.ToString();
                _response.IsSuccess = false;
            }
            return _response;
        }


    }
}
