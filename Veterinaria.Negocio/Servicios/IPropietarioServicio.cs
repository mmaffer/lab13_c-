using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public interface IPropietarioServicio
{
    Task<IEnumerable<PropietarioDto>> ObtenerTodosAsync(string? nombre, bool? conMascotas);
    Task<PropietarioDto?> ObtenerPorIdAsync(int id);

    /// <summary>Mascotas de un propietario. Devuelve null si el propietario no existe.</summary>
    Task<IEnumerable<MascotaDto>?> ObtenerMascotasAsync(int idPropietario);

    Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto);

    /// <summary>Devuelve false si el propietario no existe.</summary>
    Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto);

    /// <summary>Devuelve false si el propietario no existe.</summary>
    Task<bool> EliminarAsync(int id);
}
