using AutoMapper;
using ProductAndOrderServices.Model;
using ProductAndOrderServices.Model.Dtos;

namespace ProductAndOrderServices.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Order, OrderCreateDto>().ReverseMap();
            CreateMap<Order, OrderUpdateDto>().ReverseMap();

            CreateMap<Product, ProductCreateDto>().ReverseMap();
            CreateMap<Product, ProductUpdateDto>().ReverseMap();
        }
    }
}
