using AutoMapper;
using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models.Entities;

namespace BrasileiraoAPI.Profiles
{
    public class ProfileAutoMapper : Profile
    {
        public ProfileAutoMapper()
        {
            CreateMap<Time, TimeListarDto>();
        }
    }
}
