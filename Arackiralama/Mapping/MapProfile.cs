using AracKiralama.Models;
using AracKiralama.ViewModel;
using AutoMapper;

namespace AracKiralama.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
          
            CreateMap<AppUser,LoginViewModel>().ReverseMap();
            CreateMap<AppUser,RegisterViewModel>().ReverseMap();

          
        }
    }
}
