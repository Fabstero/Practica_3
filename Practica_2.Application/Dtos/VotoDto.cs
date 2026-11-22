namespace Practica_2.Application.Dtos;

public class VotoDto
{
    public int IdVoto { get; set; }

    public int IdVotante { get; set; }

    public string CedulaVotante { get; set; } = string.Empty;

    public string NombreVotante { get; set; } = string.Empty;

    public string ProvinciaVotante { get; set; } = string.Empty;

    public int IdPartido { get; set; }

    public string NombrePartido { get; set; } = string.Empty;

    public string SiglasPartido { get; set; } = string.Empty;

    public string NombreCandidato { get; set; } = string.Empty;

    public string? BanderaUrl { get; set; }

    public string FechaHoraVoto { get; set; } = string.Empty;
}
