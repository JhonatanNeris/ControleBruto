using AutoMapper;
using ControleBruto.Data.Dtos.Category;
using ControleBruto.Models;

namespace ControleBruto.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<CreateCategoryDto, Category>();
            CreateMap<UpdateCategoryDto, Category>();
            CreateMap<Category, ReadCategoryDto>();
        }
    }
}
