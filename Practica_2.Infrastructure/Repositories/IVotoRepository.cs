using Practica_2.Domain.Entidades;

namespace Practica_2.Infrastructure.Repositories;

public interface IVotoRepository
{
    Task<List<Voto>> ObtenerTodosConDetalleAsync();

    Task<Voto?> ObtenerPorVotanteAsync(int idVotante);

    Task<int> ContarTotalVotosAsync();

    Task<Voto> RegistrarVotoAtomicoAsync(int idVotante, int idPartido);
}
