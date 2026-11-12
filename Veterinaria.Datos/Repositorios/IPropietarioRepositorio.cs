using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public interface IPropietarioRepositorio
{
    Task<List<Propietario>> ObtenerTodosAsync(string? nombre, bool? conMascotas);
    Task<Propietario?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteAsync(int id);
    Task<bool> ExisteDocumentoAsync(string documento);
    Task<bool> TieneMascotasAsync(int id);
    Task AgregarAsync(Propietario propietario);
    Task ActualizarAsync(Propietario propietario);
    Task EliminarAsync(Propietario propietario);
}
