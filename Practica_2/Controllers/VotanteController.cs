using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.Controllers;

public class VotanteController : Controller
{
    private readonly IVotanteServicio _votanteServicio;

    public VotanteController(IVotanteServicio votanteServicio)
    {
        _votanteServicio = votanteServicio;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerVotantes()
    {
        var votantes = await _votanteServicio.ObtenerVotantesAsync();
        return Json(votantes);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerPendientes()
    {
        var votantes = await _votanteServicio.ObtenerVotantesPendientesAsync();
        return Json(votantes);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerVotantePorId(int id)
    {
        var votante = await _votanteServicio.ObtenerVotantePorIdAsync(id);
        if (votante == null)
        {
            return NotFound(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "Votante no encontrado."
            });
        }

        return Json(votante);
    }

    [HttpGet]
    public async Task<IActionResult> BuscarPorCedula(string cedula)
    {
        var votante = await _votanteServicio.ObtenerVotantePorCedulaAsync(cedula);
        if (votante == null)
        {
            return NotFound(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = $"No se encontró ningún votante con la cédula '{cedula}'."
            });
        }

        return Json(votante);
    }

    [HttpPost]
    public async Task<IActionResult> CrearVotante([FromBody] VotanteDto? votante)
    {
        if (votante == null || !ModelState.IsValid)
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

        var respuesta = await _votanteServicio.CrearVotanteAsync(votante);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpPost]
    [HttpPut]
    public async Task<IActionResult> ActualizarVotante([FromBody] VotanteDto? votante)
    {
        if (votante == null || !ModelState.IsValid)
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

        var respuesta = await _votanteServicio.ActualizarVotanteAsync(votante);
        return StatusCode(respuesta.Codigo, respuesta);
    }

    [HttpPost]
    [HttpDelete]
    public async Task<IActionResult> EliminarVotante(int id)
    {
        var respuesta = await _votanteServicio.EliminarVotanteAsync(id);
        return StatusCode(respuesta.Codigo, respuesta);
    }
}
