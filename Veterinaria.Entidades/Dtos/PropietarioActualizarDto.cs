using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

/// <summary>
/// DTO de entrada para actualizar un propietario (PUT).
/// No incluye el documento: es la identidad del propietario y no se modifica.
/// </summary>
public class PropietarioActualizarDto
{
    [Required(ErrorMessage = "Los nombres son obligatorios.")]
    [StringLength(80)]
    public string Nombres { get; set; } = null!;

    [Required(ErrorMessage = "Los apellidos son obligatorios.")]
    [StringLength(80)]
    public string Apellidos { get; set; } = null!;

    [StringLength(20)]
    public string? Telefono { get; set; }

    [EmailAddress(ErrorMessage = "El email no tiene un formato valido.")]
    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(150)]
    public string? Direccion { get; set; }
}
