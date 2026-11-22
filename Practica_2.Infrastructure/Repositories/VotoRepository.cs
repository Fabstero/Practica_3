using Microsoft.EntityFrameworkCore;
using Practica_2.Domain.Entidades;
using Practica_2.Infrastructure.Data;

namespace Practica_2.Infrastructure.Repositories;

public class VotoRepository : IVotoRepository
{
    private readonly ApplicationDbContext _context;

    public VotoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Voto>> ObtenerTodosConDetalleAsync()
    {
        return await _context.Votos
            .AsNoTracking()
            .Include(v => v.Votante)
            .Include(v => v.PartidoPolitico)
            .OrderByDescending(v => v.IdVoto)
            .ToListAsync();
    }

    public async Task<Voto?> ObtenerPorVotanteAsync(int idVotante)
    {
        return await _context.Votos
            .AsNoTracking()
            .Include(v => v.Votante)
            .Include(v => v.PartidoPolitico)
            .FirstOrDefaultAsync(v => v.IdVotante == idVotante);
    }

    public async Task<int> ContarTotalVotosAsync()
    {
        return await _context.Votos.CountAsync();
    }

    public async Task<Voto> RegistrarVotoAtomicoAsync(int idVotante, int idPartido)
    {
        await using var transaccion = await _context.Database.BeginTransactionAsync();

        try
        {
            var votante = await _context.Votantes
                .FirstOrDefaultAsync(v => v.IdVotante == idVotante)
                ?? throw new InvalidOperationException("El votante especificado no existe.");

            var nuevoVoto = new Voto
            {
                IdVotante = idVotante,
                IdPartido = idPartido
            };

            await _context.Votos.AddAsync(nuevoVoto);
            votante.HaVotado = true;

            await _context.SaveChangesAsync();
            await transaccion.CommitAsync();

            return await _context.Votos
                .AsNoTracking()
                .Include(v => v.Votante)
                .Include(v => v.PartidoPolitico)
                .FirstAsync(v => v.IdVoto == nuevoVoto.IdVoto);
        }
        catch
        {
            await transaccion.RollbackAsync();
            throw;
        }
    }
}
