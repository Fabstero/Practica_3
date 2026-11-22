using Practica_2.Domain.Entidades;

namespace Practica_2.Infrastructure.Repositories;

public interface IPartidoPoliticoRepository
{
    Task<List<PartidoPolitico>> ObtenerTodosAsync();

    Task<List<PartidoPolitico>> ObtenerActivosAsync();

    Task<PartidoPolitico?> ObtenerPorIdAsync(int idPartido);

    Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);

    Task<bool> ExisteSiglasAsync(string siglas, int? idExcluir = null);

    Task<bool> TieneVotosRegistradosAsync(int idPartido);

    Task<Dictionary<int, int>> ObtenerConteoVotosPorPartidoAsync();

    Task<PartidoPolitico> CrearAsync(PartidoPolitico partido);

    Task<bool> ActualizarAsync(PartidoPolitico partido);

    Task<bool> EliminarAsync(int idPartido);
}
