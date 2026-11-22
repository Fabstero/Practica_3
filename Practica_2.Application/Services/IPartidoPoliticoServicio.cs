using Practica_2.Application.Dtos;

namespace Practica_2.Application.Services;

public interface IPartidoPoliticoServicio
{
    Task<List<PartidoPoliticoDto>> ObtenerPartidosAsync();

    Task<List<PartidoPoliticoDto>> ObtenerPartidosActivosAsync();

    Task<PartidoPoliticoDto?> ObtenerPartidoPorIdAsync(int idPartido);

    Task<Respuesta<PartidoPoliticoDto>> CrearPartidoAsync(PartidoPoliticoDto partidoDto);

    Task<Respuesta<PartidoPoliticoDto>> ActualizarPartidoAsync(PartidoPoliticoDto partidoDto);

    Task<Respuesta<bool>> EliminarPartidoAsync(int idPartido);
}
