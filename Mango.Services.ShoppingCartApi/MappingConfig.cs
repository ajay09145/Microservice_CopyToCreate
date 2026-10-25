using AutoMapper;
using Mango.Services.ShoppingCartApi.Models;
using Mango.Services.ShoppingCartApi.Models.Dto;
using Microsoft.Extensions.Logging.Abstractions;

namespace Mango.Services.ShoppingCartApi
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                //config.CreateMap<CartDetailsDto, CartDetails>();
                config.CreateMap<CartDetails, CartDetailsDto>().ReverseMap();
                //config.CreateMap<CartHeaderDto, CartHeader>();
                config.CreateMap<CartHeader, CartHeaderDto>().ReverseMap();
            }, NullLoggerFactory.Instance);
            return mappingConfig;
        }

    }
}
