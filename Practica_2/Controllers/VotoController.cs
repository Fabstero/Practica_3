using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.Controllers;

public class VotoController : Controller
{
    private readonly IVotoServicio _votoServicio;

    public VotoController(IVotoServicio votoServicio)
    {
        _votoServicio = votoServicio;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerVotos()
    {
        var votos = await _votoServicio.ObtenerVotosAsync();
        return Json(votos);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerResumen()
    {
        var resumen = await _votoServicio.ObtenerResumenAsync();
        return Json(resumen);
    }

    [HttpPost]
    public async Task<IActionResult> EmitirVoto([FromBody] RegistrarVotoDto? registro)
    {
        if (registro == null || !ModelState.IsValid)
        {
            var primerError = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault() ?? "Seleccione un votante y un partido político válido.";

            return BadRequest(new Respuesta<object>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = primerError
            });
        }

        var respuesta = await _votoServicio.EmitirVotoAsync(registro);
        return StatusCode(respuesta.Codigo, respuesta);
    }
}
