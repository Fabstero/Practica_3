using Microsoft.EntityFrameworkCore;
using Practica_2.Domain.Entidades;
using Practica_2.Infrastructure.Data;

namespace Practica_2.Infrastructure.Repositories;

public class VotanteRepository : IVotanteRepository
{
    private readonly ApplicationDbContext _context;

    public VotanteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Votante>> ObtenerTodosAsync()
    {
        return await _context.Votantes
            .AsNoTracking()
            .OrderBy(v => v.IdVotante)
            .ToListAsync();
    }

    public async Task<List<Votante>> ObtenerPendientesAsync()
    {
        return await _context.Votantes
            .AsNoTracking()
            .Where(v => !v.HaVotado)
            .OrderBy(v => v.NombreCompleto)
            .ToListAsync();
    }

    public async Task<Votante?> ObtenerPorIdAsync(int idVotante)
    {
        return await _context.Votantes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.IdVotante == idVotante);
    }

    public async Task<Votante?> ObtenerPorCedulaAsync(string cedula)
    {
        return await _context.Votantes
            .AsNoTracking()
            .FirstOrDefaultAsync(v => v.Cedula == cedula);
    }

    public async Task<bool> ExisteCedulaAsync(string cedula, int? idExcluir = null)
    {
        return await _context.Votantes
            .AnyAsync(v => v.Cedula == cedula && (!idExcluir.HasValue || v.IdVotante != idExcluir.Value));
    }

    public async Task<bool> TieneVotoRegistradoAsync(int idVotante)
    {
        return await _context.Votos.AnyAsync(v => v.IdVotante == idVotante);
    }

    public async Task<Votante> CrearAsync(Votante votante)
    {
        await _context.Votantes.AddAsync(votante);
        await _context.SaveChangesAsync();
        return votante;
    }

    public async Task<bool> ActualizarAsync(Votante votante)
    {
        var existente = await _context.Votantes.FindAsync(votante.IdVotante);
        if (existente == null)
        {
            return false;
        }

        existente.Cedula = votante.Cedula;
        existente.NombreCompleto = votante.NombreCompleto;
        existente.FechaNacimiento = votante.FechaNacimiento;
        existente.Provincia = votante.Provincia;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarAsync(int idVotante)
    {
        var votante = await _context.Votantes.FindAsync(idVotante);
        if (votante == null)
        {
            return false;
        }

        _context.Votantes.Remove(votante);
        return await _context.SaveChangesAsync() > 0;
    }
}
