using AutoMapper;
using OrdersAPI.API.Dtos.OrderDtos;
using OrdersAPI.Core.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OrdersAPI.API.Mappings
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<CreateOrderDto, Order>();
        }
    }
}
