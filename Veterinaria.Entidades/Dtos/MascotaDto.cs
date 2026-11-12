namespace Veterinaria.Entidades.Dtos;

/// <summary>DTO de lectura: es lo que la API devuelve al cliente.</summary>
public class MascotaDto
{
    public int IdMascota { get; set; }
    public string Nombre { get; set; } = null!;
    public string Especie { get; set; } = null!;
    public string? Raza { get; set; }
    public string Sexo { get; set; } = null!;
    public DateOnly? FechaNacimiento { get; set; }
    public decimal? PesoKg { get; set; }
    public string? Microchip { get; set; }
    public bool Activo { get; set; }
    public int IdPropietario { get; set; }

    /// <summary>Nombre completo del propietario (en lugar de devolver el objeto relacionado).</summary>
    public string PropietarioNombre { get; set; } = null!;
}
