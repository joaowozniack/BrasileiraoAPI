﻿using BrasileiraoAPI.Dto;
using BrasileiraoAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BrasileiraoAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidaController : Controller
    {
        private readonly IPartidaInterface _partidaInterface;

        public PartidaController(IPartidaInterface partidaInterface)
        {
            _partidaInterface = partidaInterface;
        }

        [HttpGet("{partidaRodada}")]
        public async Task<IActionResult> ListarPartidasPorRodada(int partidaRodada)
        {
            var partidas = await _partidaInterface.BuscarPartidasPorRodada(partidaRodada);

            if (!partidas.Status)
            {
                return NotFound(partidas);
            }

            return Ok(partidas);
        }

        [HttpPost("{partidaRodada}")]
        public async Task<IActionResult> CadastrarPartidaPorRodada(int partidaRodada, PartidaCadastrarDto partidaCadastrarDto)
        {
            var partidas = await _partidaInterface.CadastrarPartidaPorRodada(partidaRodada, partidaCadastrarDto);

            if (!partidas.Status)
            {
                return BadRequest(partidas);
            }

            return Ok(partidas);
        }

    }
}
