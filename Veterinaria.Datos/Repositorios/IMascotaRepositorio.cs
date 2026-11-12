using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IMascotaRepositorio
{
    Task<List<Mascota>> ObtenerTodasAsync(string? especie, string? nombre, int? idPropietario, bool? activo);
    Task<List<Mascota>> ObtenerPorPropietarioAsync(int idPropietario);
    Task<Mascota?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteMicrochipAsync(string microchip, int? excluirIdMascota = null);
    Task AgregarAsync(Mascota mascota);
    Task ActualizarAsync(Mascota mascota);
    Task EliminarAsync(Mascota mascota);
}
