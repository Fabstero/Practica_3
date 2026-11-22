using Practica_2.Application.Dtos;

namespace Practica_2.Application.Services;

public interface IVotanteServicio
{
    Task<List<VotanteDto>> ObtenerVotantesAsync();

    Task<List<VotanteDto>> ObtenerVotantesPendientesAsync();

    Task<VotanteDto?> ObtenerVotantePorIdAsync(int idVotante);

    Task<VotanteDto?> ObtenerVotantePorCedulaAsync(string cedula);

    Task<Respuesta<VotanteDto>> CrearVotanteAsync(VotanteDto votanteDto);

    Task<Respuesta<VotanteDto>> ActualizarVotanteAsync(VotanteDto votanteDto);

    Task<Respuesta<bool>> EliminarVotanteAsync(int idVotante);
}
