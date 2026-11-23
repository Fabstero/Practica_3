using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.API.Controllers;

[ApiController]
[Route("api/votantes")]
public class VotantesController : ControllerBase
{
    private readonly IVotanteServicio _votanteServicio;

    public VotantesController(IVotanteServicio votanteServicio)
    {
        _votanteServicio = votanteServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<VotanteDto>>> ObtenerVotantes()
    {
        return Ok(await _votanteServicio.ObtenerVotantesAsync());
    }

    [HttpGet("pendientes")]
    public async Task<ActionResult<List<VotanteDto>>> ObtenerPendientes()
    {
        return Ok(await _votanteServicio.ObtenerVotantesPendientesAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VotanteDto>> ObtenerVotantePorId(int id)
    {
        var votante = await _votanteServicio.ObtenerVotantePorIdAsync(id);
        return votante == null
            ? NotFound(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "Votante no encontrado."
            })
            : Ok(votante);
    }

    [HttpGet("cedula/{cedula}")]
    public async Task<ActionResult<VotanteDto>> ObtenerVotantePorCedula(string cedula)
    {
        var votante = await _votanteServicio.ObtenerVotantePorCedulaAsync(cedula);
        return votante == null
            ? NotFound(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = $"No se encontró ningún votante con la cédula '{cedula}'."
            })
            : Ok(votante);
    }

    [HttpPost]
    public async Task<IActionResult> CrearVotante([FromBody] VotanteDto? votante)
    {
        if (votante == null || !ModelState.IsValid)
        {
            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = ObtenerPrimerError()
            });
        }

        var respuesta = await _votanteServicio.CrearVotanteAsync(votante);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpPut]
    public async Task<IActionResult> ActualizarVotante([FromBody] VotanteDto? votante)
    {
        if (votante == null || !ModelState.IsValid)
        {
            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = ObtenerPrimerError()
            });
        }

        var respuesta = await _votanteServicio.ActualizarVotanteAsync(votante);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarVotante(int id)
    {
        var respuesta = await _votanteServicio.EliminarVotanteAsync(id);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    private string ObtenerPrimerError()
    {
        return ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .FirstOrDefault() ?? "Revise los datos del formulario.";
    }
}
