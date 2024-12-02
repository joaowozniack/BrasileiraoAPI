using AutoMapper;
using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Models.Entities;
using Dapper;
using MySql.Data.MySqlClient;

namespace BrasileiraoAPI.Services
{
    public class TimeService : ITimeInterface
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public TimeService(IConfiguration configuration, IMapper mapper) 
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public async Task<ResponseModel<TimeListarDto>> BuscarTimePorId(int timeId)
        {
            ResponseModel<TimeListarDto> response = new ResponseModel<TimeListarDto>();

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var timeBanco = await connection.QueryFirstOrDefaultAsync<Time>("select * from Time where Id = @Id", new { Id = timeId });

                if (timeBanco == null)
                {
                    response.Mensagem = "Time não encontrado";
                    response.Status = false;
                    return response;
                }

                //Transformacao Mapper
                var timeMapeado = _mapper.Map<TimeListarDto>(timeBanco);

                response.Dados = timeMapeado;
                response.Mensagem = "Time encontrado com sucesso";
            }

            return response;
        }
        public async Task<ResponseModel<List<TimeListarDto>>> BuscarTimes()
        {
            ResponseModel<List<TimeListarDto>> response = new ResponseModel<List<TimeListarDto>>();

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var timesBanco = await connection.QueryAsync<Time>("select * from Time");

                if (timesBanco.Count() == 0)
                {
                    response.Mensagem = "Nenhum time encontrado";
                    response.Status = false;
                    return response;
                }

                //Transformacao Mapper
                var timesMapeado = _mapper.Map<List<TimeListarDto>>(timesBanco);

                response.Dados = timesMapeado;
                response.Mensagem = "Times encontrados com sucesso";
            }

            return response;
        }

        public async Task<ResponseModel<List<TimeListarDto>>> CadastrarTime(TimeCadastrarDto timeInserirDto)
        {
            ResponseModel<List<TimeListarDto>> response = new ResponseModel<List<TimeListarDto>>();

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var timesBanco = await connection.ExecuteAsync("insert into Time (Nome, Sigla, Escudo, Cidade, UF, AnoFundacao) " +
                                                                "values (@Nome, @Sigla, @Escudo, @Cidade, @UF, @AnoFundacao)", timeInserirDto);

                if (timesBanco == 0)
                {
                    response.Mensagem = "Erro ao cadastrar time";
                    response.Status = false;
                    return response;
                }

                var times = await ListarTimes(connection);

                var timesMapeados = _mapper.Map<List<TimeListarDto>>(times);

                response.Dados = timesMapeados;
                response.Mensagem = "Times listados com sucesso";
                response.Status = true;
            }

            return response;
        }

        private static async Task<IEnumerable<Time>> ListarTimes(MySqlConnection connection)
        {
            return await connection.QueryAsync<Time>("select * from Time");
        }

        public async Task<ResponseModel<List<TimeListarDto>>> EditarTime(TimeEditarDto timeEditarDto)
        {
            ResponseModel<List<TimeListarDto>> response = new ResponseModel<List<TimeListarDto>>();

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var timesBanco = await connection.ExecuteAsync("update Time set Nome = @Nome," +
                                                                                "Sigla = @Sigla, Escudo = @Escudo," +
                                                                                "Cidade = @Cidade, UF = @UF," +
                                                                                "AnoFundacao = @AnoFundacao where Id = @Id", timeEditarDto);

                if (timesBanco == 0)
                {
                    response.Mensagem = "Erro ao editar time";
                    response.Status = false;
                    return response;
                }

                var times = await ListarTimes(connection);

                var timesMapeados = _mapper.Map<List<TimeListarDto>>(times);

                response.Dados = timesMapeados;
                response.Mensagem = "Times listados com sucesso";
                response.Status = true;
            }

            return response;
        }

        public async Task<ResponseModel<List<TimeListarDto>>> DeletarTime(int timeId)
        {
            ResponseModel<List<TimeListarDto>> response = new ResponseModel<List<TimeListarDto>>();

            using (var connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection")))
            {
                var timesBanco = await connection.ExecuteAsync("delete from Time where Id = @Id", new { Id = timeId });

                if (timesBanco == 0)
                {
                    response.Mensagem = "Erro ao deletar time";
                    response.Status = false;
                    return response;
                }

                var times = await ListarTimes(connection);

                var timesMapeados = _mapper.Map<List<TimeListarDto>>(times);

                response.Dados = timesMapeados;
                response.Mensagem = "Times listados com sucesso";
                response.Status = true;
            }

            return response;
        }
    }
}
