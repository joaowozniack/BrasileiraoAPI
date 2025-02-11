using AutoMapper;
using Azure;
using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models;
using Dapper;
using MySql.Data.MySqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace BrasileiraoAPI.Services
{
    public class PartidaService : IPartidaInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PartidaService(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseModel<List<PartidaListarDto>>> BuscarPartidasPorRodada(int partidaRodada)
        {
            ResponseModel<List<PartidaListarDto>> response = new ResponseModel<List<PartidaListarDto>>();

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            {

                var query = @"SELECT p.Id, p.Rodada, sp.Nome as Status, 
                                     tc.Sigla as TimeCasa, tv.Sigla as TimeVisitante, 
                                     p.GolsTimeCasa, p.GolsTimeVisitante, p.Data, tc.Estadio,
                                     tc.Escudo as EscudoTimeCasa, tv.Escudo as EscudoTimeVisitante
                              FROM Partida p
                              JOIN Time tc ON p.TimeCasa = tc.Id
                              JOIN Time tv ON p.TimeVisitante = tv.Id
                              JOIN StatusPartida sp ON p.Status = sp.Id
                              WHERE p.Rodada = @Rodada";


                var partidasBanco = await connection.QueryAsync<PartidaListarDto>(query, new { Rodada = partidaRodada });

                if (partidasBanco == null || !partidasBanco.Any())
                {
                    response.Mensagem = "Nenhuma partida encontrada";
                    response.Status = false;
                    return response;
                }

                response.Dados = partidasBanco.ToList();
                response.Mensagem = "Partidas listadas com sucesso";
                response.Status = true;
            }

            return response;
        }

        public async Task<ResponseModel<List<PartidaListarDto>>> CadastrarPartidaPorRodada(int partidaRodada, PartidaCadastrarDto partidaCadastrarDto)
        {
            ResponseModel<List<PartidaListarDto>> response = new ResponseModel<List<PartidaListarDto>>();

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            {
                // Obter os times pelo sigla
                var timeCasa = await connection.QueryFirstOrDefaultAsync<Time>("SELECT * FROM Time WHERE Sigla = @Sigla", new { Sigla = partidaCadastrarDto.SiglaTimeCasa });
                var timeVisitante = await connection.QueryFirstOrDefaultAsync<Time>("SELECT * FROM Time WHERE Sigla = @Sigla", new { Sigla = partidaCadastrarDto.SiglaTimeVisitante });

                if (timeCasa == null || timeVisitante == null)
                {
                    response.Mensagem = "Time da casa ou visitante não encontrado";
                    response.Status = false;
                    return response;
                }

                // Inserir a nova partida
                var query = @"INSERT INTO Partida (Rodada, Status, TimeCasa, TimeVisitante, Data, Estadio)
                      VALUES (@Rodada, @Status, @TimeCasa, @TimeVisitante, @Data, @Estadio)";

                var parameters = new
                {
                    Rodada = partidaRodada,
                    Status = 1, // Status inicial
                    TimeCasa = timeCasa.Id,
                    TimeVisitante = timeVisitante.Id,
                    Data = partidaCadastrarDto.Data,
                    Estadio = timeCasa.Estadio
                };

                var result = await connection.ExecuteAsync(query, parameters);

                if (result == 0)
                {
                    response.Mensagem = "Erro ao cadastrar a partida";
                    response.Status = false;
                    return response;
                }

                var partidas = await ListarPartidas(connection);

                var partidasMapeadas = _mapper.Map<List<PartidaListarDto>>(partidas);

                response.Dados = partidasMapeadas;
                response.Mensagem = "Partidas listadas com sucesso";
                response.Status = true;
            }

            return response;
        }

        private static async Task<IEnumerable<PartidaListarDto>> ListarPartidas(MySqlConnection connection)
        {
            var query = @"SELECT p.Id, p.Rodada, sp.Nome as Status, 
                             tc.Sigla as TimeCasa, tv.Sigla as TimeVisitante, 
                             p.GolsTimeCasa, p.GolsTimeVisitante, p.Data, tc.Estadio
                      FROM Partida p
                      JOIN Time tc ON p.TimeCasa = tc.Id
                      JOIN Time tv ON p.TimeVisitante = tv.Id
                      JOIN StatusPartida sp ON p.Status = sp.Id";


            var partidas = await connection.QueryAsync<PartidaListarDto>(query);

            return partidas;
        }

        public async Task<ResponseModel<List<PartidaListarDto>>> ListarPartidasPagina()
        {
            ResponseModel<List<PartidaListarDto>> response = new ResponseModel<List<PartidaListarDto>>();

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            {
                var query = @"SELECT p.Id, p.Rodada, sp.Nome as Status, 
                                     tc.Sigla as TimeCasa, tv.Sigla as TimeVisitante, 
                                     p.GolsTimeCasa, p.GolsTimeVisitante, p.Data, tc.Estadio,
                                     tc.Escudo as EscudoTimeCasa, tv.Escudo as EscudoTimeVisitante
                              FROM Partida p
                              JOIN Time tc ON p.TimeCasa = tc.Id
                              JOIN Time tv ON p.TimeVisitante = tv.Id
                              JOIN StatusPartida sp ON p.Status = sp.Id";


                var partidasBanco = await connection.QueryAsync<PartidaListarDto>(query);

                if (partidasBanco == null || !partidasBanco.Any())
                {
                    response.Mensagem = "Nenhuma partida encontrada";
                    response.Status = false;
                    return response;
                }

                response.Dados = partidasBanco.ToList();
                response.Mensagem = "Partidas listadas com sucesso";
                response.Status = true;
            }

            return response;
        }

        public async Task<ResponseModel<List<PartidaListarDto>>> AtualizarPartidaComPlacar(int idPartida, int golstimeCasa, int golsTimeVisitante)
        {
            ResponseModel<List<PartidaListarDto>> response = new ResponseModel<List<PartidaListarDto>>();

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            {
                var query = @"UPDATE Partida SET GolsTimeCasa = @GolsTimeCasa, GolsTimeVisitante = @GolsTimeVisitante, Status = @Status WHERE Id = @Id";

                var parameters = new
                {
                    Id = idPartida,
                    GolsTimeCasa = golstimeCasa,
                    GolsTimeVisitante = golsTimeVisitante,
                    Status = 3,
                };

                var result = await connection.ExecuteAsync(query, parameters);

                if (result == 0)
                {
                    response.Mensagem = "Erro ao atualizar a partida";
                    response.Status = false;
                    return response;
                }

                var partida = await connection.QueryFirstOrDefaultAsync<PartidaListarDto>("SELECT TimeCasa, TimeVisitante, GolsTimeCasa," +
                                                                                          "GolsTimeVisitante FROM Partida " +
                                                                                          "WHERE Id = @Id", new { Id = idPartida });
                var timeCasa = await connection.QueryFirstOrDefaultAsync<TimeListarDto>("SELECT * FROM Time WHERE Id = @Id", new { Id = partida.TimeCasa });
                var timeVisitante = await connection.QueryFirstOrDefaultAsync<TimeListarDto>("SELECT * FROM Time WHERE Id = @Id", new { Id = partida.TimeVisitante });

                if (golstimeCasa > golsTimeVisitante)
                {
                    //Time da casa ganhou
                    timeCasa.Vitorias += 1;
                    timeCasa.Pontos += 3;
                    timeVisitante.Derrotas += 1;
                }
                else if (golstimeCasa < golsTimeVisitante)
                {
                    //Time visitante ganhou
                    timeVisitante.Vitorias += 1;
                    timeVisitante.Pontos += 3;
                    timeCasa.Derrotas += 1;
                }
                else
                {
                    //Empate
                    timeCasa.Empates += 1;
                    timeVisitante.Empates += 1;
                    timeCasa.Pontos += 1;
                    timeVisitante.Pontos += 1;
                }

                //Atualizar jogos, gols e saldo de gols
                timeCasa.Jogos += 1;
                timeCasa.GolsPro += golstimeCasa;
                timeCasa.GolsContra += golsTimeVisitante;
                timeCasa.SaldoGols = timeCasa.GolsPro - timeCasa.GolsContra;

                timeVisitante.Jogos += 1;
                timeVisitante.GolsPro += golsTimeVisitante;
                timeVisitante.GolsContra += golstimeCasa;
                timeVisitante.SaldoGols = timeVisitante.GolsPro - timeVisitante.GolsContra;

                // Atualizar os dados no banco de dados
                var updateTimeQuery = @"UPDATE Time SET Vitorias = @Vitorias, Empates = @Empates, Derrotas = @Derrotas, 
                                Pontos = @Pontos, GolsPro = @GolsPro, GolsContra = @GolsContra, SaldoGols = @SaldoGols, Jogos = @Jogos 
                                WHERE Id = @Id";

                await connection.ExecuteAsync(updateTimeQuery, timeCasa);
                await connection.ExecuteAsync(updateTimeQuery, timeVisitante);

                var partidas = await ListarPartidas(connection);

                var partidasMapeadas = _mapper.Map<List<PartidaListarDto>>(partidas);

                response.Dados = partidasMapeadas;
                response.Mensagem = "Partidas listadas com sucesso";
                response.Status = true;
            }

            return response;
        }
    }
}
