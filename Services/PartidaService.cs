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
                var timeCasa = await ObterTimePorSigla(connection, partidaCadastrarDto.SiglaTimeCasa);
                var timeVisitante = await ObterTimePorSigla(connection, partidaCadastrarDto.SiglaTimeVisitante);

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

        public async Task<ResponseModel<PartidaListarDto>> AtualizarPartidaComPlacar(PartidaListarDto partidaDto)
        {
            ResponseModel<PartidaListarDto> response = new ResponseModel<PartidaListarDto>();

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
            
            try
            {

                var partidaAtual = await ObterPartidaAtual(connection, partidaDto.Id);
                if (partidaAtual == null)
                {
                    response.Mensagem = "Partida não encontrada";
                    response.Status = false;
                    return response;
                }

                var timeCasa = await ObterTimePorSigla(connection, partidaDto.TimeCasa);
                var timeVisitante = await ObterTimePorSigla(connection, partidaDto.TimeVisitante);
                if (timeCasa == null || timeVisitante == null)
                {
                    response.Mensagem = "Time da casa ou visitante não encontrado";
                    response.Status = false;
                    return response;
                }

                // Reverter as estatísticas dos times com base no resultado anterior
                if (partidaAtual.Status == "2")
                {
                    ReverterEstatiscaticasTime(partidaAtual, timeCasa, timeVisitante);
                }

                if (partidaAtual.Status == "3")
                {
                    response.Mensagem = "Partida já finalizada";
                    response.Status = false;
                    return response;
                }

                AtualizarEstatisticasTime(partidaDto, timeCasa, timeVisitante);

                // Atualizar o status para "Em andamento"
                var statusEmAndamento = await ObterStatus(connection, "Em andamento");
                if (statusEmAndamento == null)
                {
                    response.Mensagem = "Status 'Em andamento' não encontrado";
                    response.Status = false;
                    return response;
                }

                partidaDto.Status = statusEmAndamento.Nome;

                var result = await AtualizarPartida(connection, partidaDto, statusEmAndamento.Id);
                if (result == 0)
                {
                    response.Mensagem = $"Erro ao atualizar a partida com Id {partidaDto.Id}";
                    response.Status = false;
                    return response;
                }

                await AtualizarTime(connection, timeCasa);
                await AtualizarTime(connection, timeVisitante);

                response.Dados = partidaDto;
                response.Mensagem = "Partida atualizada com sucesso";
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Mensagem = $"Erro ao atualizar a partida: {ex.Message}";
                response.Status = false;
            }

            return response;
        }

        public async Task<ResponseModel<PartidaListarDto>> FinalizarPartida(PartidaListarDto partidaDto)
        {
            ResponseModel<PartidaListarDto> response = new ResponseModel<PartidaListarDto>();

            using var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            try
            {
                var partidaAtual = await ObterPartidaAtual(connection, partidaDto.Id);
                if (partidaAtual == null)
                {
                    response.Mensagem = "Partida não encontrada";
                    response.Status = false;
                    return response;
                }

                // Atualizar status da partida para "Encerrado"
                var statusEncerrado = await ObterStatus(connection, "Encerrado");
                if (statusEncerrado == null)
                {
                    response.Mensagem = "Status 'Encerrado' não encontrado";
                    response.Status = false;
                    return response;
                }

                partidaDto.Status = statusEncerrado.Nome;

                var result = await AtualizarPartida(connection, partidaAtual, statusEncerrado.Id);
                if (result == 0)
                {
                    response.Mensagem = $"Erro ao atualizar a partida com Id {partidaDto.Id}";
                    response.Status = false;
                    return response;
                }

                response.Dados = partidaDto;
                response.Mensagem = "Partida finalizada com sucesso";
                response.Status = true;
            }
            catch (Exception ex)
            {
                response.Mensagem = $"Erro ao finalizar a partida: {ex.Message}";
                response.Status = false;
            }

            return response;
        }

        private async Task<PartidaListarDto> ObterPartidaAtual(MySqlConnection connection, int partidaId)
        {
            return await connection.QueryFirstOrDefaultAsync<PartidaListarDto>("SELECT * FROM Partida WHERE Id = @Id", new { Id = partidaId });
        }

        private async Task<TimeListarDto> ObterTimePorSigla(MySqlConnection connection, string sigla)
        {
            return await connection.QueryFirstOrDefaultAsync<TimeListarDto>("SELECT * FROM Time WHERE Sigla = @Sigla", new { Sigla = sigla });
        }

        private void ReverterEstatiscaticasTime(PartidaListarDto partidaAtual, TimeListarDto timeCasa, TimeListarDto timeVisitante)
        {
            if (partidaAtual.GolsTimeCasa > partidaAtual.GolsTimeVisitante)
            {
                // Time da casa ganhou anteriormente
                timeCasa.Vitorias -= 1;
                timeCasa.Pontos -= 3;
                timeVisitante.Derrotas -= 1;
            }
            else if (partidaAtual.GolsTimeCasa < partidaAtual.GolsTimeVisitante)
            {
                // Time visitante ganhou anteriormente
                timeVisitante.Vitorias -= 1;
                timeVisitante.Pontos -= 3;
                timeCasa.Derrotas -= 1;
            }
            else
            {
                // Empate anteriormente
                timeCasa.Empates -= 1;
                timeVisitante.Empates -= 1;
                timeCasa.Pontos -= 1;
                timeVisitante.Pontos -= 1;
            }

            // Reverter jogos, gols e saldo de gol
            timeCasa.Jogos -= 1;
            timeCasa.GolsPro -= partidaAtual.GolsTimeCasa;
            timeCasa.GolsContra -= partidaAtual.GolsTimeVisitante;
            timeCasa.SaldoGols = timeCasa.GolsPro - timeCasa.GolsContra;

            timeVisitante.Jogos -= 1;
            timeVisitante.GolsPro -= partidaAtual.GolsTimeVisitante;
            timeVisitante.GolsContra -= partidaAtual.GolsTimeCasa;
            timeVisitante.SaldoGols = timeVisitante.GolsPro - timeVisitante.GolsContra;
        }

        private void AtualizarEstatisticasTime(PartidaListarDto partidaDto, TimeListarDto timeCasa, TimeListarDto timeVisitante)
        {
            if (partidaDto.GolsTimeCasa > partidaDto.GolsTimeVisitante)
            {
                // Time da casa ganhou
                timeCasa.Vitorias += 1;
                timeCasa.Pontos += 3;
                timeVisitante.Derrotas += 1;
            }
            else if (partidaDto.GolsTimeCasa < partidaDto.GolsTimeVisitante)
            {
                // Time visitante ganhou
                timeVisitante.Vitorias += 1;
                timeVisitante.Pontos += 3;
                timeCasa.Derrotas += 1;
            }
            else
            {
                // Empate
                timeCasa.Empates += 1;
                timeVisitante.Empates += 1;
                timeCasa.Pontos += 1;
                timeVisitante.Pontos += 1;
            }

            // Atualizar jogos, gols e saldo de gols
            timeCasa.Jogos += 1;
            timeCasa.GolsPro += partidaDto.GolsTimeCasa;
            timeCasa.GolsContra += partidaDto.GolsTimeVisitante;
            timeCasa.SaldoGols = timeCasa.GolsPro - timeCasa.GolsContra;

            timeVisitante.Jogos += 1;
            timeVisitante.GolsPro += partidaDto.GolsTimeVisitante;
            timeVisitante.GolsContra += partidaDto.GolsTimeCasa;
            timeVisitante.SaldoGols = timeVisitante.GolsPro - timeVisitante.GolsContra;
        }

        // Mais genérico
        private async Task<StatusPartida> ObterStatus(MySqlConnection connection, string nomeStatus)
        {
            return await connection.QueryFirstOrDefaultAsync<StatusPartida>("SELECT Id, Nome FROM StatusPartida WHERE Nome = @Nome", new {Nome = nomeStatus});
        }

        private async Task<int> AtualizarPartida(MySqlConnection connection, PartidaListarDto partidaDto, int statusId)
        { 
            var query = @"UPDATE Partida SET GolsTimeCasa = @GolsTimeCasa, GolsTimeVisitante = @GolsTimeVisitante, Status = @Status WHERE Id = @Id";
            var parameters = new
            {
                Id = partidaDto.Id,
                GolsTimeCasa = partidaDto.GolsTimeCasa,
                GolsTimeVisitante = partidaDto.GolsTimeVisitante,
                Status = statusId
            };
            return await connection.ExecuteAsync(query, parameters);
        }

        private async Task AtualizarTime(MySqlConnection connection, TimeListarDto time)
        {
            var query = @"UPDATE Time SET Vitorias = @Vitorias, Empates = @Empates, Derrotas = @Derrotas, Pontos = @Pontos, GolsPro = @GolsPro, GolsContra = @GolsContra, SaldoGols = @SaldoGols, Jogos = @Jogos WHERE Id = @Id";
            await connection.ExecuteAsync(query, time);
        }
    }
}