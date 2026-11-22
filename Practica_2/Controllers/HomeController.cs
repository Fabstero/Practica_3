using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Practica_2.Application.Services;
using Practica_2.Models;

namespace Practica_2.Controllers;

public class HomeController : Controller
{
    private readonly IVotoServicio _votoServicio;

    public HomeController(IVotoServicio votoServicio)
    {
        _votoServicio = votoServicio;
    }

    public async Task<IActionResult> Index()
    {
        var resumen = await _votoServicio.ObtenerResumenAsync();
        return View(resumen);
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerResumen()
    {
        var resumen = await _votoServicio.ObtenerResumenAsync();
        return Json(resumen);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
