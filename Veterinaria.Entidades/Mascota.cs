namespace Veterinaria.Entidades;

public partial class Mascota
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

    public virtual Propietario IdPropietarioNavigation { get; set; } = null!;
}
