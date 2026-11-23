using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.API.Controllers;

[ApiController]
[Route("api/votos")]
public class VotosController : ControllerBase
{
    private readonly IVotoServicio _votoServicio;

    public VotosController(IVotoServicio votoServicio)
    {
        _votoServicio = votoServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<VotoDto>>> ObtenerVotos()
    {
        return Ok(await _votoServicio.ObtenerVotosAsync());
    }

    [HttpGet("resumen")]
    public async Task<ActionResult<ResumenVotacionDto>> ObtenerResumen()
    {
        return Ok(await _votoServicio.ObtenerResumenAsync());
    }

    [HttpPost]
    public async Task<IActionResult> EmitirVoto([FromBody] RegistrarVotoDto? registro)
    {
        if (registro == null || !ModelState.IsValid)
        {
            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .FirstOrDefault() ?? "Seleccione un votante y un partido político válido."
            });
        }

        var respuesta = await _votoServicio.EmitirVotoAsync(registro);
        return StatusCode(respuesta.Codigo, respuesta);
    }
}
