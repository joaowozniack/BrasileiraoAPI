
using AutoMapper;
using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Profiles
{
    public class PartidaAutoMapper : Profile
    {
        public PartidaAutoMapper() 
        {
            CreateMap<Partida, PartidaListarDto>();
        }
    }
}
