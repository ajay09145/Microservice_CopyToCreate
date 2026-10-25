
using Mango.Web.Models;
using Mango.Web.Services.IService;
using Mango.Web.Utility;
using Newtonsoft.Json;

namespace Mango.Web.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IBaseService _baseService;
        public ProductService(IBaseService baseService)
        {
                _baseService = baseService;
        }


        public async Task<ResponseDto?> GetAllProductsAsync()
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.ProductAPIBase + "/api/product"
            });
        }        

        public async Task<ResponseDto?> GetProductAsync(string couponCode)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.ProductAPIBase + "/api/product/GetByCode" + couponCode
            });
        }

        public async Task<ResponseDto?> GetProductByIdAsync(int Id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.GET,
                Url = SD.ProductAPIBase + "/api/product/" + Id
            });
        }

        public async Task<ResponseDto?> DeleteProductAsync(int Id)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.DELETE,
                Url = SD.ProductAPIBase + "/api/product/" + Id
            });
        }

        public async Task<ResponseDto?> CreateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.POST,
                //Data = JsonConvert.SerializeObject(couponDto),
                Data = productDto,
                Url = SD.ProductAPIBase + "/api/product"
            }); 
        }

        public async Task<ResponseDto?> UpdateProductAsync(ProductDto productDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                APIType = SD.APITypes.PUT,
                Data = productDto,
                //Data = JsonConvert.SerializeObject(productDto),
                Url = SD.ProductAPIBase + "/api/product"
            });
        }
    }
}
