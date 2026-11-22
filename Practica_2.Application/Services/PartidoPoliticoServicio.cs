using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Practica_2.Application.Dtos;
using Practica_2.Domain.Entidades;
using Practica_2.Infrastructure.Repositories;

namespace Practica_2.Application.Services;

public class PartidoPoliticoServicio : IPartidoPoliticoServicio
{
    private readonly IPartidoPoliticoRepository _partidoRepository;
    private readonly IMapper _mapper;

    public PartidoPoliticoServicio(IPartidoPoliticoRepository partidoRepository, IMapper mapper)
    {
        _partidoRepository = partidoRepository;
        _mapper = mapper;
    }

    public async Task<List<PartidoPoliticoDto>> ObtenerPartidosAsync()
    {
        var partidos = await _partidoRepository.ObtenerTodosAsync();
        var conteos = await _partidoRepository.ObtenerConteoVotosPorPartidoAsync();

        var resultado = _mapper.Map<List<PartidoPoliticoDto>>(partidos);
        foreach (var dto in resultado)
        {
            dto.CantidadVotos = conteos.GetValueOrDefault(dto.IdPartido, 0);
        }

        return resultado;
    }

    public async Task<List<PartidoPoliticoDto>> ObtenerPartidosActivosAsync()
    {
        var partidos = await _partidoRepository.ObtenerActivosAsync();
        var conteos = await _partidoRepository.ObtenerConteoVotosPorPartidoAsync();

        var resultado = _mapper.Map<List<PartidoPoliticoDto>>(partidos);
        foreach (var dto in resultado)
        {
            dto.CantidadVotos = conteos.GetValueOrDefault(dto.IdPartido, 0);
        }

        return resultado;
    }

    public async Task<PartidoPoliticoDto?> ObtenerPartidoPorIdAsync(int idPartido)
    {
        var partido = await _partidoRepository.ObtenerPorIdAsync(idPartido);
        if (partido == null)
        {
            return null;
        }

        var conteos = await _partidoRepository.ObtenerConteoVotosPorPartidoAsync();
        var dto = _mapper.Map<PartidoPoliticoDto>(partido);
        dto.CantidadVotos = conteos.GetValueOrDefault(dto.IdPartido, 0);
        return dto;
    }

    public async Task<Respuesta<PartidoPoliticoDto>> CrearPartidoAsync(PartidoPoliticoDto partidoDto)
    {
        var validacion = ValidarCampos(partidoDto);
        if (validacion != null)
        {
            return validacion;
        }

        var nombreLimpio = partidoDto.Nombre.Trim();
        var siglasLimpias = partidoDto.Siglas.Trim().ToUpperInvariant();

        if (await _partidoRepository.ExisteNombreAsync(nombreLimpio))
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"Ya existe un partido político registrado con el nombre '{nombreLimpio}'."
            };
        }

        if (await _partidoRepository.ExisteSiglasAsync(siglasLimpias))
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"Ya existe un partido político registrado con las siglas '{siglasLimpias}'."
            };
        }

        var entidad = new PartidoPolitico
        {
            Nombre = nombreLimpio,
            Siglas = siglasLimpias,
            BanderaUrl = string.IsNullOrWhiteSpace(partidoDto.BanderaUrl) ? null : partidoDto.BanderaUrl.Trim(),
            NombreCandidato = partidoDto.NombreCandidato.Trim(),
            Activo = partidoDto.Activo
        };

        try
        {
            var creado = await _partidoRepository.CrearAsync(entidad);
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = true,
                Codigo = 201,
                Mensaje = "Partido político registrado correctamente.",
                Dato = _mapper.Map<PartidoPoliticoDto>(creado)
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se pudo registrar el partido debido a un conflicto de unicidad en nombre o siglas."
            };
        }
    }

    public async Task<Respuesta<PartidoPoliticoDto>> ActualizarPartidoAsync(PartidoPoliticoDto partidoDto)
    {
        if (partidoDto.IdPartido <= 0)
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "El identificador del partido político no es válido."
            };
        }

        var validacion = ValidarCampos(partidoDto);
        if (validacion != null)
        {
            return validacion;
        }

        var existente = await _partidoRepository.ObtenerPorIdAsync(partidoDto.IdPartido);
        if (existente == null)
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "El partido político solicitado no existe."
            };
        }

        var nombreLimpio = partidoDto.Nombre.Trim();
        var siglasLimpias = partidoDto.Siglas.Trim().ToUpperInvariant();

        if (await _partidoRepository.ExisteNombreAsync(nombreLimpio, partidoDto.IdPartido))
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"Ya existe otro partido político con el nombre '{nombreLimpio}'."
            };
        }

        if (await _partidoRepository.ExisteSiglasAsync(siglasLimpias, partidoDto.IdPartido))
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"Ya existe otro partido político con las siglas '{siglasLimpias}'."
            };
        }

        var entidad = new PartidoPolitico
        {
            IdPartido = partidoDto.IdPartido,
            Nombre = nombreLimpio,
            Siglas = siglasLimpias,
            BanderaUrl = string.IsNullOrWhiteSpace(partidoDto.BanderaUrl) ? null : partidoDto.BanderaUrl.Trim(),
            NombreCandidato = partidoDto.NombreCandidato.Trim(),
            Activo = partidoDto.Activo
        };

        try
        {
            await _partidoRepository.ActualizarAsync(entidad);
            var actualizado = await ObtenerPartidoPorIdAsync(partidoDto.IdPartido);

            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = true,
                Codigo = 200,
                Mensaje = "Partido político actualizado correctamente.",
                Dato = actualizado
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se pudo actualizar el partido debido a un conflicto de datos."
            };
        }
    }

    public async Task<Respuesta<bool>> EliminarPartidoAsync(int idPartido)
    {
        var existente = await _partidoRepository.ObtenerPorIdAsync(idPartido);
        if (existente == null)
        {
            return new Respuesta<bool>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "El partido político solicitado no existe.",
                Dato = false
            };
        }

        if (await _partidoRepository.TieneVotosRegistradosAsync(idPartido))
        {
            return new Respuesta<bool>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se puede eliminar este partido político porque tiene votos registrados (restricción ON DELETE RESTRICT). Puede desactivarlo en su lugar.",
                Dato = false
            };
        }

        try
        {
            var eliminado = await _partidoRepository.EliminarAsync(idPartido);
            return new Respuesta<bool>
            {
                EsCorrecto = eliminado,
                Codigo = eliminado ? 200 : 404,
                Mensaje = eliminado ? "Partido político eliminado correctamente." : "No se encontró el partido político a eliminar.",
                Dato = eliminado
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<bool>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se puede eliminar el partido político porque tiene votos asociados (ON DELETE RESTRICT).",
                Dato = false
            };
        }
    }

    private static Respuesta<PartidoPoliticoDto>? ValidarCampos(PartidoPoliticoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nombre) ||
            string.IsNullOrWhiteSpace(dto.Siglas) ||
            string.IsNullOrWhiteSpace(dto.NombreCandidato))
        {
            return new Respuesta<PartidoPoliticoDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "El nombre, las siglas y el nombre del candidato son obligatorios."
            };
        }

        return null;
    }
}
