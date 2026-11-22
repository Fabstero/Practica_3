using System.ComponentModel.DataAnnotations;

namespace Practica_2.Application.Dtos;

public class RegistrarVotoDto
{
    public int? IdVotante { get; set; }

    public string? Cedula { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un partido político válido.")]
    public int IdPartido { get; set; }
}
