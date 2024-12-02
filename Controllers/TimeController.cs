using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BrasileiraoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeController : ControllerBase
    {
        private readonly ITimeInterface _timeInterface;

        public TimeController(ITimeInterface timeInterface)
        {
            _timeInterface = timeInterface;   
        }

        [HttpGet]
        public async Task<IActionResult> ListarTimes()
        {
            var times = await _timeInterface.BuscarTimes();

            if (!times.Status)
            {
                return NotFound(times);
            }

            return Ok(times);
        }

        [HttpGet("{timeId}")]
        public async Task<IActionResult> ListarTimePorId(int timeId)
        {
            var time = await _timeInterface.BuscarTimePorId(timeId);

            if (!time.Status)
            {
                return NotFound(time);
            }

            return Ok(time);
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarTime(TimeCadastrarDto timeInserirDto)
        {
            var time = await _timeInterface.CadastrarTime(timeInserirDto);

            if (!time.Status)
            {
                return BadRequest(time);
            }

            return Ok(time);
        }

        [HttpPut]
        public async Task<IActionResult> EditarTime(TimeEditarDto timeEditarDto)
        {
            var time = await _timeInterface.EditarTime(timeEditarDto);

            if (!time.Status)
            {
                return BadRequest(time);
            }

            return Ok(time);
        }

        [HttpDelete]
        public async Task<IActionResult> DeletarTime(int timeId)
        {
            var time = await _timeInterface.DeletarTime(timeId);

            if (!time.Status)
            {
                return BadRequest(time);
            }

            return Ok(time);
        }
    }
}
