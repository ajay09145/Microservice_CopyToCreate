using Mango.Web.Models;

namespace Mango.Web.Services.IService
{
    public interface IProductService
    {
        Task<ResponseDto?> GetProductAsync(string couponCode);
        Task<ResponseDto?> GetAllProductsAsync();
        Task<ResponseDto?> GetProductByIdAsync(int Id);
        Task<ResponseDto?> CreateProductAsync(ProductDto productDto);
        Task<ResponseDto?> UpdateProductAsync(ProductDto productDto);
        Task<ResponseDto?> DeleteProductAsync(int Id);

       
    }
}
