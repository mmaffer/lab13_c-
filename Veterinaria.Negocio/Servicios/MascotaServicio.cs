using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public class MascotaServicio : IMascotaServicio
{
    private readonly IMascotaRepositorio _mascotas;
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMapper _mapper;

    public MascotaServicio(
        IMascotaRepositorio mascotas,
        IPropietarioRepositorio propietarios,
        IMapper mapper)
    {
        _mascotas = mascotas;
        _propietarios = propietarios;
        _mapper = mapper;
    }

    public async Task<IEnumerable<MascotaDto>> ObtenerTodasAsync(string? especie, string? nombre, int? idPropietario, bool? activo)
    {
        var lista = await _mascotas.ObtenerTodasAsync(especie, nombre, idPropietario, activo);
        return lista.Select(m => _mapper.Map<MascotaDto>(m)).ToList();
    }

    public async Task<MascotaDto?> ObtenerPorIdAsync(int id)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        return mascota is null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    public async Task<MascotaDto> CrearAsync(MascotaCrearDto dto)
    {
        await ValidarReglasAsync(dto.IdPropietario, dto.FechaNacimiento, dto.Microchip, null);

        var mascota = _mapper.Map<Mascota>(dto);
        await _mascotas.AgregarAsync(mascota);

        // Se vuelve a leer para que venga cargado el propietario (nombre en el DTO).
        var creada = await _mascotas.ObtenerPorIdAsync(mascota.IdMascota);
        return _mapper.Map<MascotaDto>(creada);
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        if (mascota is null)
        {
            return false;
        }

        await ValidarReglasAsync(dto.IdPropietario, dto.FechaNacimiento, dto.Microchip, id);

        // Map(origen, destino): copia los datos del DTO sobre la entidad existente.
        _mapper.Map(dto, mascota);
        await _mascotas.ActualizarAsync(mascota);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mascota = await _mascotas.ObtenerPorIdAsync(id);
        if (mascota is null)
        {
            return false;
        }

        await _mascotas.EliminarAsync(mascota);
        return true;
    }

    /// <summary>Reglas de negocio compartidas por crear y actualizar una mascota.</summary>
    private async Task ValidarReglasAsync(int idPropietario, DateOnly? fechaNacimiento, string? microchip, int? idMascota)
    {
        // Regla de negocio 3: la mascota debe pertenecer a un propietario registrado.
        if (!await _propietarios.ExisteAsync(idPropietario))
        {
            throw new ReglaNegocioException($"El propietario con id {idPropietario} no existe.");
        }

        // Regla de negocio 4: la fecha de nacimiento no puede estar en el futuro.
        if (fechaNacimiento.HasValue && fechaNacimiento.Value > DateOnly.FromDateTime(DateTime.Today))
        {
            throw new ReglaNegocioException("La fecha de nacimiento no puede ser posterior a hoy.");
        }

        // Regla de negocio 5: el microchip (si se informa) identifica a una sola mascota.
        if (!string.IsNullOrWhiteSpace(microchip) &&
            await _mascotas.ExisteMicrochipAsync(microchip, idMascota))
        {
            throw new ReglaNegocioException($"El microchip {microchip} ya esta asignado a otra mascota.");
        }
    }
}
