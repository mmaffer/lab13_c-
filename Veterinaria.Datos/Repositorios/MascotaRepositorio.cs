using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class MascotaRepositorio : IMascotaRepositorio
{
    private readonly VeterinariaContext _context;

    public MascotaRepositorio(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<List<Mascota>> ObtenerTodasAsync(string? especie, string? nombre, int? idPropietario, bool? activo)
    {
        IQueryable<Mascota> consulta = _context.Mascotas
            .AsNoTracking()
            .Include(m => m.IdPropietarioNavigation);

        if (!string.IsNullOrWhiteSpace(especie))
        {
            var patronEspecie = especie.Trim();
            consulta = consulta.Where(m => EF.Functions.ILike(m.Especie, patronEspecie));
        }

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var patronNombre = $"%{nombre.Trim()}%";
            consulta = consulta.Where(m => EF.Functions.ILike(m.Nombre, patronNombre));
        }

        if (idPropietario.HasValue)
        {
            consulta = consulta.Where(m => m.IdPropietario == idPropietario.Value);
        }

        if (activo.HasValue)
        {
            consulta = consulta.Where(m => m.Activo == activo.Value);
        }

        return await consulta
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<List<Mascota>> ObtenerPorPropietarioAsync(int idPropietario)
    {
        return await _context.Mascotas
            .AsNoTracking()
            .Include(m => m.IdPropietarioNavigation)
            .Where(m => m.IdPropietario == idPropietario)
            .OrderBy(m => m.Nombre)
            .ToListAsync();
    }

    public async Task<Mascota?> ObtenerPorIdAsync(int id)
    {
        return await _context.Mascotas
            .Include(m => m.IdPropietarioNavigation)
            .FirstOrDefaultAsync(m => m.IdMascota == id);
    }

    public async Task<bool> ExisteMicrochipAsync(string microchip, int? excluirIdMascota = null)
    {
        return await _context.Mascotas.AnyAsync(m =>
            m.Microchip == microchip &&
            (excluirIdMascota == null || m.IdMascota != excluirIdMascota));
    }

    public async Task AgregarAsync(Mascota mascota)
    {
        await _context.Mascotas.AddAsync(mascota);
        await _context.SaveChangesAsync();
    }

    // La entidad ya viene rastreada por el contexto (ObtenerPorIdAsync): basta con guardar los cambios.
    public async Task ActualizarAsync(Mascota mascota)
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Mascota mascota)
    {
        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
    }
}
