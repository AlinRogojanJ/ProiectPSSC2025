    using AutoMapper;
    using ProiectPSSC2025.DTOs;
    using ProiectPSSC2025.Models;
using ProiectPSSC2025.Models.DTOs;

namespace ProiectPSSC2025.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
            CreateMap<User, UserDTO>().ReverseMap();  
        }
    }

}
