using System.ComponentModel.DataAnnotations;

namespace Practica_2.Application.Dtos;

public class PartidoPoliticoDto
{
    public int IdPartido { get; set; }

    [Required(ErrorMessage = "El nombre del partido es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre no puede exceder los 150 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Las siglas son obligatorias.")]
    [StringLength(20, ErrorMessage = "Las siglas no pueden exceder los 20 caracteres.")]
    public string Siglas { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La URL de la bandera no puede exceder los 500 caracteres.")]
    public string? BanderaUrl { get; set; }

    [Required(ErrorMessage = "El nombre del candidato es obligatorio.")]
    [StringLength(150, ErrorMessage = "El nombre del candidato no puede exceder los 150 caracteres.")]
    public string NombreCandidato { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;

    public int CantidadVotos { get; set; }
}
