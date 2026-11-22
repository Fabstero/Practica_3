using Microsoft.EntityFrameworkCore;
using Practica_2.Domain.Entidades;
using Practica_2.Infrastructure.Data;

namespace Practica_2.Infrastructure.Repositories;

public class PartidoPoliticoRepository : IPartidoPoliticoRepository
{
    private readonly ApplicationDbContext _context;

    public PartidoPoliticoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PartidoPolitico>> ObtenerTodosAsync()
    {
        return await _context.PartidosPoliticos
            .AsNoTracking()
            .OrderBy(p => p.IdPartido)
            .ToListAsync();
    }

    public async Task<List<PartidoPolitico>> ObtenerActivosAsync()
    {
        return await _context.PartidosPoliticos
            .AsNoTracking()
            .Where(p => p.Activo)
            .OrderBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<PartidoPolitico?> ObtenerPorIdAsync(int idPartido)
    {
        return await _context.PartidosPoliticos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.IdPartido == idPartido);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
    {
        var normalizado = nombre.Trim().ToLower();
        return await _context.PartidosPoliticos
            .AnyAsync(p => p.Nombre.ToLower() == normalizado && (!idExcluir.HasValue || p.IdPartido != idExcluir.Value));
    }

    public async Task<bool> ExisteSiglasAsync(string siglas, int? idExcluir = null)
    {
        var normalizado = siglas.Trim().ToUpper();
        return await _context.PartidosPoliticos
            .AnyAsync(p => p.Siglas.ToUpper() == normalizado && (!idExcluir.HasValue || p.IdPartido != idExcluir.Value));
    }

    public async Task<bool> TieneVotosRegistradosAsync(int idPartido)
    {
        return await _context.Votos.AnyAsync(v => v.IdPartido == idPartido);
    }

    public async Task<Dictionary<int, int>> ObtenerConteoVotosPorPartidoAsync()
    {
        return await _context.Votos
            .AsNoTracking()
            .GroupBy(v => v.IdPartido)
            .Select(g => new { IdPartido = g.Key, Cantidad = g.Count() })
            .ToDictionaryAsync(x => x.IdPartido, x => x.Cantidad);
    }

    public async Task<PartidoPolitico> CrearAsync(PartidoPolitico partido)
    {
        await _context.PartidosPoliticos.AddAsync(partido);
        await _context.SaveChangesAsync();
        return partido;
    }

    public async Task<bool> ActualizarAsync(PartidoPolitico partido)
    {
        var existente = await _context.PartidosPoliticos.FindAsync(partido.IdPartido);
        if (existente == null)
        {
            return false;
        }

        existente.Nombre = partido.Nombre;
        existente.Siglas = partido.Siglas;
        existente.BanderaUrl = partido.BanderaUrl;
        existente.NombreCandidato = partido.NombreCandidato;
        existente.Activo = partido.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int idPartido)
    {
        var partido = await _context.PartidosPoliticos.FindAsync(idPartido);
        if (partido == null)
        {
            return false;
        }

        _context.PartidosPoliticos.Remove(partido);
        return await _context.SaveChangesAsync() > 0;
    }
}
