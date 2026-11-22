namespace Practica_2.Application.Dtos;

public class ResumenVotacionDto
{
    public int TotalVotantes { get; set; }

    public int TotalVotosEmitidos { get; set; }

    public int VotantesPendientes { get; set; }

    public double PorcentajeParticipacion { get; set; }

    public int TotalPartidos { get; set; }

    public int PartidosActivos { get; set; }

    public List<ResultadoPartidoDto> ResultadosPorPartido { get; set; } = new();
}

public class ResultadoPartidoDto
{
    public int IdPartido { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Siglas { get; set; } = string.Empty;

    public string NombreCandidato { get; set; } = string.Empty;

    public string? BanderaUrl { get; set; }

    public bool Activo { get; set; }

    public int TotalVotos { get; set; }

    public double Porcentaje { get; set; }
}
