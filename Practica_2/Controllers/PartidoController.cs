using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.Controllers;

public class PartidoController : Controller
{
    private readonly IPartidoPoliticoServicio _partidoServicio;

    public PartidoController(IPartidoPoliticoServicio partidoServicio)
    {
        _partidoServicio = partidoServicio;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPartidos()
    {
        var partidos = await _partidoServicio.ObtenerPartidosAsync();
        return Json(partidos);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPartidosActivos()
    {
        var partidos = await _partidoServicio.ObtenerPartidosActivosAsync();
        return Json(partidos);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPartidoPorId(int id)
    {
        var partido = await _partidoServicio.ObtenerPartidoPorIdAsync(id);
        if (partido == null)
        {
            return NotFound(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "Partido político no encontrado."
            });
        }

        return Json(partido);
    }

    [HttpPost]
    public async Task<IActionResult> CrearPartido([FromBody] PartidoPoliticoDto? partido)
    {
        if (partido == null || !ModelState.IsValid)
        {
            var primerError = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Revise los datos del formulario.";

            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = primerError
            });
        }

        var respuesta = await _partidoServicio.CrearPartidoAsync(partido);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpPost]
    [HttpPut]
    public async Task<IActionResult> ActualizarPartido([FromBody] PartidoPoliticoDto? partido)
    {
        if (partido == null || !ModelState.IsValid)
        {
            var primerError = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Revise los datos del formulario.";

            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = primerError
            });
        }

        var respuesta = await _partidoServicio.ActualizarPartidoAsync(partido);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpPost]
    [HttpDelete]
    public async Task<IActionResult> EliminarPartido(int id)
    {
        var respuesta = await _partidoServicio.EliminarPartidoAsync(id);
        return StatusCode(respuesta.Codigo, respuesta);
    }
}
