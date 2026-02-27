using AutoMapper;
using ControleBruto.Data.Dtos;
using ControleBruto.Models;

namespace ControleBruto.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<CreateAccountDto, Account>();
            CreateMap<UpdateAccountDto, Account>();
            CreateMap<Account, ReadAccountDto>();
        }
    }
}
