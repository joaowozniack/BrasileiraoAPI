using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Services
{
    public interface ITimeInterface
    {
        Task<ResponseModel<List<TimeListarDto>>> BuscarTimes();
        Task<ResponseModel<TimeListarDto>> BuscarTimePorId(int timeId);
        Task<ResponseModel<List<TimeListarDto>>> CadastrarTime(TimeCadastrarDto timeInserirDto);
        Task<ResponseModel<List<TimeListarDto>>> EditarTime(TimeEditarDto timeEditarDto);
        Task<ResponseModel<List<TimeListarDto>>> DeletarTime(int timeId);
    }
}
