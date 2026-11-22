using System.ComponentModel.DataAnnotations;

namespace Practica_2.Application.Dtos;

public class VotanteDto
{
    public int IdVotante { get; set; }

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [StringLength(30, ErrorMessage = "La cédula no puede exceder los 30 caracteres.")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre completo es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre completo no puede exceder los 150 caracteres.")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    [RegularExpression(@"^\d{4}-\d{2}-\d{2}$", ErrorMessage = "La fecha de nacimiento debe usar el formato YYYY-MM-DD.")]
    public string FechaNacimiento { get; set; } = string.Empty;

    [Required(ErrorMessage = "La provincia es obligatoria.")]
    [StringLength(60, ErrorMessage = "La provincia no puede exceder los 60 caracteres.")]
    public string Provincia { get; set; } = string.Empty;

    public bool HaVotado { get; set; }
}
