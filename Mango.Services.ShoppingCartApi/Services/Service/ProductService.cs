using Mango.Services.ShoppingCartApi.Models.Dto;
using Mango.Services.ShoppingCartApi.Services.IService;
using Newtonsoft.Json;

namespace Mango.Services.ShoppingCartApi.Services.Service
{
    public class ProductService : IProductService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetAllProducts()
        {
            var client = _httpClientFactory.CreateClient("ProductAPI");
            //var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");

            var apiContent = await response.Content.ReadAsStringAsync();
            var resp = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (resp.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(resp.Result));

            }
            return new List<ProductDto>();

        }
    }
}
