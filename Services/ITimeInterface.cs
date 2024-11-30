using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Services
{
    public interface ITimeInterface
    {
        Task<List<TimeListarDto>> ListarTimes();
    }
}
