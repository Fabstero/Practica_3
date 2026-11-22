using Microsoft.EntityFrameworkCore;
using Practica_2.Domain.Entidades;

namespace Practica_2.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task InicializarAsync(ApplicationDbContext context)
    {
        await context.Database.EnsureCreatedAsync();
        await context.Database.ExecuteSqlRawAsync("PRAGMA foreign_keys = ON;");

        if (!await context.Votantes.AnyAsync())
        {
            var votantesIniciales = new List<Votante>
            {
                new()
                {
                    Cedula = "101110111",
                    NombreCompleto = "Carlos Rodríguez Mora",
                    FechaNacimiento = "1995-04-12",
                    Provincia = "San José",
                    HaVotado = false
                },
                new()
                {
                    Cedula = "302220222",
                    NombreCompleto = "María Fernanda Solano",
                    FechaNacimiento = "1998-09-23",
                    Provincia = "Cartago",
                    HaVotado = false
                },
                new()
                {
                    Cedula = "203330333",
                    NombreCompleto = "Andrés Jiménez Castro",
                    FechaNacimiento = "1990-01-15",
                    Provincia = "Alajuela",
                    HaVotado = false
                },
                new()
                {
                    Cedula = "404440444",
                    NombreCompleto = "Sofía Vargas Brenes",
                    FechaNacimiento = "2001-11-30",
                    Provincia = "Heredia",
                    HaVotado = false
                }
            };

            await context.Votantes.AddRangeAsync(votantesIniciales);
            await context.SaveChangesAsync();
        }

        if (!await context.PartidosPoliticos.AnyAsync())
        {
            var partidosIniciales = new List<PartidoPolitico>
            {
                new()
                {
                    Nombre = "Partido Innovación Tecnológica",
                    Siglas = "PIT",
                    NombreCandidato = "Elena Gómez",
                    Activo = true
                },
                new()
                {
                    Nombre = "Movimiento Desarrollo Nacional",
                    Siglas = "MDN",
                    NombreCandidato = "Roberto Alvarado",
                    Activo = true
                },
                new()
                {
                    Nombre = "Alianza Progreso Verde",
                    Siglas = "APV",
                    NombreCandidato = "Lucía Méndez",
                    Activo = true
                }
            };

            await context.PartidosPoliticos.AddRangeAsync(partidosIniciales);
            await context.SaveChangesAsync();
        }
    }
}
