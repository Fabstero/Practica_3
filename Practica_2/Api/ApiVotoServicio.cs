using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.Api;

public sealed class ApiVotoServicio : ApiClientBase, IVotoServicio
{
    public ApiVotoServicio(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public Task<List<VotoDto>> ObtenerVotosAsync()
    {
        return GetRequiredAsync<List<VotoDto>>("api/votos");
    }

    public Task<ResumenVotacionDto> ObtenerResumenAsync()
    {
        return GetRequiredAsync<ResumenVotacionDto>("api/votos/resumen");
    }

    public Task<Respuesta<VotoDto>> EmitirVotoAsync(RegistrarVotoDto registro)
    {
        return SendAsync<VotoDto>(HttpMethod.Post, "api/votos", registro);
    }
}
