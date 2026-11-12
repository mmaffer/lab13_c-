using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

/// <summary>DTO de entrada para registrar un propietario (POST).</summary>
public class PropietarioCrearDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(80)]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(80)]
    public string Apellidos { get; set; } = null!;

    [Required(ErrorMessage = "El documento es obligatorio.")]
    [StringLength(15, MinimumLength = 8, ErrorMessage = "El documento debe tener entre 8 y 15 caracteres.")]
    public string Documento { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El email no tiene un formato valido.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(150)]
    public string? Direccion { get; set; }
}
