namespace Practica_2.Domain.Entidades;

public class Votante
{
    public int IdVotante { get; set; }

    public string Cedula { get; set; } = string.Empty;

    public string NombreCompleto { get; set; } = string.Empty;

    public string FechaNacimiento { get; set; } = string.Empty;

    public string Provincia { get; set; } = string.Empty;

    public bool HaVotado { get; set; }

    public virtual Voto? Voto { get; set; }
}
