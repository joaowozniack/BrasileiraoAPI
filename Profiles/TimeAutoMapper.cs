using AutoMapper;
using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Profiles
{
    public class TimeAutoMapper : Profile
    {
        public TimeAutoMapper()
        {
            CreateMap<Time, TimeListarDto>();
        }
    }
}
