using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

/// <summary>DTO de entrada para actualizar una mascota (PUT).</summary>
public class MascotaActualizarDto
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(60)]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La especie es obligatoria.")]
    [StringLength(30)]
    public string Especie { get; set; } = null!;

    [StringLength(60)]
    public string? Raza { get; set; }

    [Required(ErrorMessage = "El sexo es obligatorio.")]
    [RegularExpression("^[MH]$", ErrorMessage = "El sexo debe ser 'M' (macho) o 'H' (hembra).")]
    public string Sexo { get; set; } = null!;

    public DateOnly? FechaNacimiento { get; set; }

    [Range(0.01, 200, ErrorMessage = "El peso debe estar entre 0.01 y 200 kg.")]
    public decimal? PesoKg { get; set; }

    [StringLength(20)]
    public string? Microchip { get; set; }

    public bool Activo { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Debe indicar un propietario valido.")]
    public int IdPropietario { get; set; }
}
