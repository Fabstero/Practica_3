using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Practica_2.Application.Dtos;
using Practica_2.Domain.Entidades;
using Practica_2.Infrastructure.Repositories;

namespace Practica_2.Application.Services;

public class VotanteServicio : IVotanteServicio
{
    private readonly IVotanteRepository _votanteRepository;
    private readonly IMapper _mapper;

    public VotanteServicio(IVotanteRepository votanteRepository, IMapper mapper)
    {
        _votanteRepository = votanteRepository;
        _mapper = mapper;
    }

    public async Task<List<VotanteDto>> ObtenerVotantesAsync()
    {
        var votantes = await _votanteRepository.ObtenerTodosAsync();
        return _mapper.Map<List<VotanteDto>>(votantes);
    }

    public async Task<List<VotanteDto>> ObtenerVotantesPendientesAsync()
    {
        var votantes = await _votanteRepository.ObtenerPendientesAsync();
        return _mapper.Map<List<VotanteDto>>(votantes);
    }

    public async Task<VotanteDto?> ObtenerVotantePorIdAsync(int idVotante)
    {
        var votante = await _votanteRepository.ObtenerPorIdAsync(idVotante);
        return votante == null ? null : _mapper.Map<VotanteDto>(votante);
    }

    public async Task<VotanteDto?> ObtenerVotantePorCedulaAsync(string cedula)
    {
        if (string.IsNullOrWhiteSpace(cedula))
        {
            return null;
        }

        var votante = await _votanteRepository.ObtenerPorCedulaAsync(cedula.Trim());
        return votante == null ? null : _mapper.Map<VotanteDto>(votante);
    }

    public async Task<Respuesta<VotanteDto>> CrearVotanteAsync(VotanteDto votanteDto)
    {
        var validacion = ValidarCampos(votanteDto);
        if (validacion != null)
        {
            return validacion;
        }

        var cedulaLimpia = votanteDto.Cedula.Trim();
        if (await _votanteRepository.ExisteCedulaAsync(cedulaLimpia))
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"Ya existe un votante registrado con la cédula '{cedulaLimpia}'."
            };
        }

        var entidad = new Votante
        {
            Cedula = cedulaLimpia,
            NombreCompleto = votanteDto.NombreCompleto.Trim(),
            FechaNacimiento = votanteDto.FechaNacimiento.Trim(),
            Provincia = votanteDto.Provincia.Trim(),
            HaVotado = false
        };

        try
        {
            var creado = await _votanteRepository.CrearAsync(entidad);
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = true,
                Codigo = 201,
                Mensaje = "Votante registrado correctamente.",
                Dato = _mapper.Map<VotanteDto>(creado)
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se pudo registrar el votante debido a un conflicto de unicidad en la base de datos."
            };
        }
    }

    public async Task<Respuesta<VotanteDto>> ActualizarVotanteAsync(VotanteDto votanteDto)
    {
        if (votanteDto.IdVotante <= 0)
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "El identificador del votante no es válido."
            };
        }

        var validacion = ValidarCampos(votanteDto);
        if (validacion != null)
        {
            return validacion;
        }

        var existente = await _votanteRepository.ObtenerPorIdAsync(votanteDto.IdVotante);
        if (existente == null)
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "El votante solicitado no existe."
            };
        }

        var cedulaLimpia = votanteDto.Cedula.Trim();
        if (await _votanteRepository.ExisteCedulaAsync(cedulaLimpia, votanteDto.IdVotante))
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"Ya existe otro votante registrado con la cédula '{cedulaLimpia}'."
            };
        }

        var entidad = new Votante
        {
            IdVotante = votanteDto.IdVotante,
            Cedula = cedulaLimpia,
            NombreCompleto = votanteDto.NombreCompleto.Trim(),
            FechaNacimiento = votanteDto.FechaNacimiento.Trim(),
            Provincia = votanteDto.Provincia.Trim(),
            HaVotado = existente.HaVotado
        };

        try
        {
            await _votanteRepository.ActualizarAsync(entidad);
            var actualizado = await _votanteRepository.ObtenerPorIdAsync(votanteDto.IdVotante);

            return new Respuesta<VotanteDto>
            {
                EsCorrecto = true,
                Codigo = 200,
                Mensaje = "Votante actualizado correctamente.",
                Dato = actualizado == null ? null : _mapper.Map<VotanteDto>(actualizado)
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se pudo actualizar el votante debido a un conflicto de datos."
            };
        }
    }

    public async Task<Respuesta<bool>> EliminarVotanteAsync(int idVotante)
    {
        var existente = await _votanteRepository.ObtenerPorIdAsync(idVotante);
        if (existente == null)
        {
            return new Respuesta<bool>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "El votante solicitado no existe.",
                Dato = false
            };
        }

        if (existente.HaVotado || await _votanteRepository.TieneVotoRegistradoAsync(idVotante))
        {
            return new Respuesta<bool>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se puede eliminar este votante porque ya cuenta con un voto registrado (restricción ON DELETE RESTRICT).",
                Dato = false
            };
        }

        try
        {
            var eliminado = await _votanteRepository.EliminarAsync(idVotante);
            return new Respuesta<bool>
            {
                EsCorrecto = eliminado,
                Codigo = eliminado ? 200 : 404,
                Mensaje = eliminado ? "Votante eliminado correctamente." : "No se encontró el votante a eliminar.",
                Dato = eliminado
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<bool>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "No se puede eliminar el votante porque tiene votos asociados (ON DELETE RESTRICT).",
                Dato = false
            };
        }
    }

    private static Respuesta<VotanteDto>? ValidarCampos(VotanteDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Cedula) ||
            string.IsNullOrWhiteSpace(dto.NombreCompleto) ||
            string.IsNullOrWhiteSpace(dto.FechaNacimiento) ||
            string.IsNullOrWhiteSpace(dto.Provincia))
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "Todos los campos del votante (cédula, nombre completo, fecha de nacimiento y provincia) son obligatorios."
            };
        }

        if (!DateOnly.TryParseExact(
                dto.FechaNacimiento.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var fecha))
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "La fecha de nacimiento debe tener el formato válido YYYY-MM-DD."
            };
        }

        if (fecha > DateOnly.FromDateTime(DateTime.Today))
        {
            return new Respuesta<VotanteDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "La fecha de nacimiento no puede ser futura."
            };
        }

        return null;
    }
}
