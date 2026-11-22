using Practica_2.Application.Dtos;

namespace Practica_2.Application.Services;

public interface IVotoServicio
{
    Task<List<VotoDto>> ObtenerVotosAsync();

    Task<ResumenVotacionDto> ObtenerResumenAsync();

    Task<Respuesta<VotoDto>> EmitirVotoAsync(RegistrarVotoDto registro);
}
