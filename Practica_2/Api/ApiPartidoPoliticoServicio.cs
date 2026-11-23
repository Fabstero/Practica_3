using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.Api;

public sealed class ApiPartidoPoliticoServicio : ApiClientBase, IPartidoPoliticoServicio
{
    public ApiPartidoPoliticoServicio(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public Task<List<PartidoPoliticoDto>> ObtenerPartidosAsync()
    {
        return GetRequiredAsync<List<PartidoPoliticoDto>>("api/partidos");
    }

    public Task<List<PartidoPoliticoDto>> ObtenerPartidosActivosAsync()
    {
        return GetRequiredAsync<List<PartidoPoliticoDto>>("api/partidos/activos");
    }

    public Task<PartidoPoliticoDto?> ObtenerPartidoPorIdAsync(int idPartido)
    {
        return GetOptionalAsync<PartidoPoliticoDto>($"api/partidos/{idPartido}");
    }

    public Task<Respuesta<PartidoPoliticoDto>> CrearPartidoAsync(PartidoPoliticoDto partidoDto)
    {
        return SendAsync<PartidoPoliticoDto>(HttpMethod.Post, "api/partidos", partidoDto);
    }

    public Task<Respuesta<PartidoPoliticoDto>> ActualizarPartidoAsync(PartidoPoliticoDto partidoDto)
    {
        return SendAsync<PartidoPoliticoDto>(HttpMethod.Put, "api/partidos", partidoDto);
    }

    public Task<Respuesta<bool>> EliminarPartidoAsync(int idPartido)
    {
        return SendAsync<bool>(HttpMethod.Delete, $"api/partidos/{idPartido}");
    }
}
