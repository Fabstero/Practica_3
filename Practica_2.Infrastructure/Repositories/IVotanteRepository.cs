using Practica_2.Domain.Entidades;

namespace Practica_2.Infrastructure.Repositories;

public interface IVotanteRepository
{
    Task<List<Votante>> ObtenerTodosAsync();

    Task<List<Votante>> ObtenerPendientesAsync();

    Task<Votante?> ObtenerPorIdAsync(int idVotante);

    Task<Votante?> ObtenerPorCedulaAsync(string cedula);

    Task<bool> ExisteCedulaAsync(string cedula, int? idExcluir = null);

    Task<bool> TieneVotoRegistradoAsync(int idVotante);

    Task<Votante> CrearAsync(Votante votante);

    Task<bool> ActualizarAsync(Votante votante);

    Task<bool> EliminarAsync(int idVotante);
}
