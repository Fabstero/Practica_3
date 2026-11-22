namespace Practica_2.Domain.Entidades;

public class Voto
{
    public int IdVoto { get; set; }

    public int IdVotante { get; set; }

    public int IdPartido { get; set; }

    public string FechaHoraVoto { get; set; } = null!;

    public virtual Votante? Votante { get; set; }

    public virtual PartidoPolitico? PartidoPolitico { get; set; }
}
