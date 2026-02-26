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
            CreateMap<Account, ReadAccountDto>();
            CreateMap<UpdateAccountDto, Account>();

        }

    }
}
