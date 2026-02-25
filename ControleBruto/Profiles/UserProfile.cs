using ControleBruto.Data.Dtos;
using ControleBruto.Models;
using AutoMapper;

namespace ControleBruto.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<CreateUserDto, User>();
        }
    }
}
