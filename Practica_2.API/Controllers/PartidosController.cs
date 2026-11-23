using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.API.Controllers;

[ApiController]
[Route("api/partidos")]
public class PartidosController : ControllerBase
{
    private readonly IPartidoPoliticoServicio _partidoServicio;

    public PartidosController(IPartidoPoliticoServicio partidoServicio)
    {
        _partidoServicio = partidoServicio;
    }

    [HttpGet]
    public async Task<ActionResult<List<PartidoPoliticoDto>>> ObtenerPartidos()
    {
        return Ok(await _partidoServicio.ObtenerPartidosAsync());
    }

    [HttpGet("activos")]
    public async Task<ActionResult<List<PartidoPoliticoDto>>> ObtenerPartidosActivos()
    {
        return Ok(await _partidoServicio.ObtenerPartidosActivosAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartidoPoliticoDto>> ObtenerPartidoPorId(int id)
    {
        var partido = await _partidoServicio.ObtenerPartidoPorIdAsync(id);
        return partido == null
            ? NotFound(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "Partido político no encontrado."
            })
            : Ok(partido);
    }

    [HttpPost]
    public async Task<IActionResult> CrearPartido([FromBody] PartidoPoliticoDto? partido)
    {
        if (partido == null || !ModelState.IsValid)
        {
            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = ObtenerPrimerError()
            });
        }

        var respuesta = await _partidoServicio.CrearPartidoAsync(partido);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpPut]
    public async Task<IActionResult> ActualizarPartido([FromBody] PartidoPoliticoDto? partido)
    {
        if (partido == null || !ModelState.IsValid)
        {
            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = ObtenerPrimerError()
            });
        }

        var respuesta = await _partidoServicio.ActualizarPartidoAsync(partido);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> EliminarPartido(int id)
    {
        var respuesta = await _partidoServicio.EliminarPartidoAsync(id);
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
