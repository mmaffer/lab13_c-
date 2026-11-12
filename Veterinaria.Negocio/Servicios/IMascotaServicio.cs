using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Servicios;

public interface IMascotaServicio
{
    Task<IEnumerable<MascotaDto>> ObtenerTodasAsync(string? especie, string? nombre, int? idPropietario, bool? activo);
    Task<MascotaDto?> ObtenerPorIdAsync(int id);
    Task<MascotaDto> CrearAsync(MascotaCrearDto dto);

    /// <summary>Devuelve false si la mascota no existe.</summary>
    Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto);

    /// <summary>Devuelve false si la mascota no existe.</summary>
    Task<bool> EliminarAsync(int id);
}
