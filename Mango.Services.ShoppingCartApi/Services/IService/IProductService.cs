using Mango.Services.ShoppingCartApi.Models.Dto;

namespace Mango.Services.ShoppingCartApi.Services.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProducts();
    }
}
