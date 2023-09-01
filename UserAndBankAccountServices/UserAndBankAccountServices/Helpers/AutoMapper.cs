using AutoMapper;
using UserAndBankAccountServices.Model.Dtos;
using UserAndBankAccountServices.Models;
using UserAndBankAccountServices.Models.Dtos;

namespace UserAndBankAccountServices.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<User, UserCreateDto>().ReverseMap();
            CreateMap<User, UserUpdateDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();

            CreateMap<BankAccount, BankAccountDto>().ReverseMap();
        }
    }
}
