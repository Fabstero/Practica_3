using Practica_2.Application.Dtos;
using Practica_2.Application.Services;

namespace Practica_2.Api;

public sealed class ApiVotanteServicio : ApiClientBase, IVotanteServicio
{
    public ApiVotanteServicio(HttpClient httpClient)
        : base(httpClient)
    {
    }

    public Task<List<VotanteDto>> ObtenerVotantesAsync()
    {
        return GetRequiredAsync<List<VotanteDto>>("api/votantes");
    }

    public Task<List<VotanteDto>> ObtenerVotantesPendientesAsync()
    {
        return GetRequiredAsync<List<VotanteDto>>("api/votantes/pendientes");
    }

    public Task<VotanteDto?> ObtenerVotantePorIdAsync(int idVotante)
    {
        return GetOptionalAsync<VotanteDto>($"api/votantes/{idVotante}");
    }

    public Task<VotanteDto?> ObtenerVotantePorCedulaAsync(string cedula)
    {
        return GetOptionalAsync<VotanteDto>($"api/votantes/cedula/{Uri.EscapeDataString(cedula)}");
    }

    public Task<Respuesta<VotanteDto>> CrearVotanteAsync(VotanteDto votanteDto)
    {
        return SendAsync<VotanteDto>(HttpMethod.Post, "api/votantes", votanteDto);
    }

    public Task<Respuesta<VotanteDto>> ActualizarVotanteAsync(VotanteDto votanteDto)
    {
        return SendAsync<VotanteDto>(HttpMethod.Put, "api/votantes", votanteDto);
    }

    public Task<Respuesta<bool>> EliminarVotanteAsync(int idVotante)
    {
        return SendAsync<bool>(HttpMethod.Delete, $"api/votantes/{idVotante}");
    }
}
