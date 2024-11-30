using BrasileiraoAPI.Dto;

namespace BrasileiraoAPI.Services
{
    public class TimeService : ITimeInterface
    {
        private readonly IConfiguration _configuration;

        public TimeService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public Task<List<TimeListarDto>> ListarTimes()
        {
            throw new NotImplementedException();
        }
    }
}
