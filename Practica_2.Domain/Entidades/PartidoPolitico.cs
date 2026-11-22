namespace Practica_2.Domain.Entidades;

public class PartidoPolitico
{
    public int IdPartido { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Siglas { get; set; } = string.Empty;

    public string? BanderaUrl { get; set; }

    public string NombreCandidato { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
