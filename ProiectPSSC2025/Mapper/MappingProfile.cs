    using AutoMapper;
    using ProiectPSSC2025.DTOs;
    using ProiectPSSC2025.Models;

namespace ProiectPSSC2025.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDTO>().ReverseMap();
        }
    }

}
