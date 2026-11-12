namespace Veterinaria.Entidades.Dtos;

/// <summary>DTO de lectura: es lo que la API devuelve al cliente.</summary>
public class PropietarioDto
{
    public int IdPropietario { get; set; }
    public string Nombres { get; set; } = null!;
    public string Apellidos { get; set; } = null!;
    public string Documento { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public DateTime FechaRegistro { get; set; }

    /// <summary>Dato calculado: cuantas mascotas tiene el propietario.</summary>
    public int CantidadMascotas { get; set; }
}
