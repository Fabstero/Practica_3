using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Practica_2.Application.Dtos;
using Practica_2.Domain.Entidades;
using Practica_2.Infrastructure.Repositories;

namespace Practica_2.Application.Services;

public class VotoServicio : IVotoServicio
{
    private readonly IVotoRepository _votoRepository;
    private readonly IVotanteRepository _votanteRepository;
    private readonly IPartidoPoliticoRepository _partidoRepository;
    private readonly IMapper _mapper;

    public VotoServicio(
        IVotoRepository votoRepository,
        IVotanteRepository votanteRepository,
        IPartidoPoliticoRepository partidoRepository,
        IMapper mapper)
    {
        _votoRepository = votoRepository;
        _votanteRepository = votanteRepository;
        _partidoRepository = partidoRepository;
        _mapper = mapper;
    }

    public async Task<List<VotoDto>> ObtenerVotosAsync()
    {
        var votos = await _votoRepository.ObtenerTodosConDetalleAsync();
        return _mapper.Map<List<VotoDto>>(votos);
    }

    public async Task<ResumenVotacionDto> ObtenerResumenAsync()
    {
        var votantes = await _votanteRepository.ObtenerTodosAsync();
        var partidos = await _partidoRepository.ObtenerTodosAsync();
        var conteos = await _partidoRepository.ObtenerConteoVotosPorPartidoAsync();
        var totalVotos = await _votoRepository.ContarTotalVotosAsync();

        var totalVotantes = votantes.Count;
        var votantesQueVotaron = votantes.Count(v => v.HaVotado);
        var pendientes = Math.Max(0, totalVotantes - votantesQueVotaron);
        var participacion = totalVotantes > 0
            ? Math.Round((double)totalVotos * 100.0 / totalVotantes, 1)
            : 0.0;

        var resultados = partidos
            .Select(p =>
            {
                var cantidad = conteos.GetValueOrDefault(p.IdPartido, 0);
                var porcentaje = totalVotos > 0
                    ? Math.Round((double)cantidad * 100.0 / totalVotos, 1)
                    : 0.0;

                return new ResultadoPartidoDto
                {
                    IdPartido = p.IdPartido,
                    Nombre = p.Nombre,
                    Siglas = p.Siglas,
                    NombreCandidato = p.NombreCandidato,
                    BanderaUrl = p.BanderaUrl,
                    Activo = p.Activo,
                    TotalVotos = cantidad,
                    Porcentaje = porcentaje
                };
            })
            .OrderByDescending(r => r.TotalVotos)
            .ThenBy(r => r.Nombre)
            .ToList();

        return new ResumenVotacionDto
        {
            TotalVotantes = totalVotantes,
            TotalVotosEmitidos = totalVotos,
            VotantesPendientes = pendientes,
            PorcentajeParticipacion = participacion,
            TotalPartidos = partidos.Count,
            PartidosActivos = partidos.Count(p => p.Activo),
            ResultadosPorPartido = resultados
        };
    }

    public async Task<Respuesta<VotoDto>> EmitirVotoAsync(RegistrarVotoDto registro)
    {
        if (registro.IdPartido <= 0)
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "Debe seleccionar un partido político válido para emitir el voto."
            };
        }

        Votante? votante = null;

        if (registro.IdVotante.HasValue && registro.IdVotante.Value > 0)
        {
            votante = await _votanteRepository.ObtenerPorIdAsync(registro.IdVotante.Value);
        }
        else if (!string.IsNullOrWhiteSpace(registro.Cedula))
        {
            votante = await _votanteRepository.ObtenerPorCedulaAsync(registro.Cedula.Trim());
        }
        else
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = "Debe indicar la cédula o seleccionar un votante habilitado."
            };
        }

        if (votante == null)
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "No se encontró ningún votante registrado con los datos proporcionados."
            };
        }

        if (votante.HaVotado || await _votanteRepository.TieneVotoRegistradoAsync(votante.IdVotante))
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = $"El votante {votante.NombreCompleto} (cédula {votante.Cedula}) ya emitió su voto y no puede votar dos veces."
            };
        }

        var partido = await _partidoRepository.ObtenerPorIdAsync(registro.IdPartido);
        if (partido == null)
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 404,
                Mensaje = "El partido político seleccionado no existe."
            };
        }

        if (!partido.Activo)
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 400,
                Mensaje = $"El partido '{partido.Nombre}' se encuentra inactivo y no puede recibir votos."
            };
        }

        try
        {
            var votoCreado = await _votoRepository.RegistrarVotoAtomicoAsync(votante.IdVotante, partido.IdPartido);
            return new Respuesta<VotoDto>
            {
                EsCorrecto = true,
                Codigo = 201,
                Mensaje = $"Voto registrado exitosamente para {partido.Nombre} ({partido.Siglas}).",
                Dato = _mapper.Map<VotoDto>(votoCreado)
            };
        }
        catch (DbUpdateException)
        {
            return new Respuesta<VotoDto>
            {
                EsCorrecto = false,
                Codigo = 409,
                Mensaje = "El votante ya registró su voto previamente."
            };
        }
    }
}
