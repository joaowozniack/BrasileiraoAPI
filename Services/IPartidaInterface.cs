using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models;

namespace BrasileiraoAPI.Services
{
    public interface IPartidaInterface
    {
        Task<ResponseModel<List<PartidaListarDto>>> BuscarPartidasPorRodada(int partidaRodada);
        Task<ResponseModel<List<PartidaListarDto>>> ListarPartidasPagina();
        Task<ResponseModel<List<PartidaListarDto>>> CadastrarPartidaPorRodada(int partidaRodada, PartidaCadastrarDto partidaCadastrarDto);
        Task<ResponseModel<List<PartidaListarDto>>> AtualizarPartidaComPlacar(int idPartida, int golsTimeCasa, int golsTimeVisitante);
    }
}
