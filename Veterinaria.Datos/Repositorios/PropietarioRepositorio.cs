using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Entidades;

namespace Veterinaria.Datos.Repositorios;

public class PropietarioRepositorio : IPropietarioRepositorio
{
    private readonly VeterinariaContext _context;

    public PropietarioRepositorio(VeterinariaContext context)
    {
        _context = context;
    }

    public async Task<List<Propietario>> ObtenerTodosAsync(string? nombre, bool? conMascotas)
    {
        IQueryable<Propietario> consulta = _context.Propietarios
            .AsNoTracking()
            .Include(p => p.Mascotas);

        if (!string.IsNullOrWhiteSpace(nombre))
        {
            var patron = $"%{nombre.Trim()}%";
            consulta = consulta.Where(p => EF.Functions.ILike(p.Nombres + " " + p.Apellidos, patron));
        }

        if (conMascotas.HasValue)
        {
            consulta = conMascotas.Value
                ? consulta.Where(p => p.Mascotas.Any())
                : consulta.Where(p => !p.Mascotas.Any());
        }

        return await consulta
            .OrderBy(p => p.Apellidos)
            .ThenBy(p => p.Nombres)
            .ToListAsync();
    }

    public async Task<Propietario?> ObtenerPorIdAsync(int id)
    {
        return await _context.Propietarios
            .Include(p => p.Mascotas)
            .FirstOrDefaultAsync(p => p.IdPropietario == id);
    }

    public async Task<bool> ExisteAsync(int id)
    {
        return await _context.Propietarios.AnyAsync(p => p.IdPropietario == id);
    }

    public async Task<bool> ExisteDocumentoAsync(string documento)
    {
        return await _context.Propietarios.AnyAsync(p => p.Documento == documento);
    }

    public async Task<bool> TieneMascotasAsync(int id)
    {
        return await _context.Mascotas.AnyAsync(m => m.IdPropietario == id);
    }

    public async Task AgregarAsync(Propietario propietario)
    {
        await _context.Propietarios.AddAsync(propietario);
        await _context.SaveChangesAsync();
    }

    // La entidad ya viene rastreada por el contexto (ObtenerPorIdAsync): basta con guardar los cambios.
    public async Task ActualizarAsync(Propietario propietario)
    {
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Propietario propietario)
    {
        _context.Propietarios.Remove(propietario);
        await _context.SaveChangesAsync();
    }
}
