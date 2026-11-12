using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public class PropietarioServicio : IPropietarioServicio
{
    private readonly IPropietarioRepositorio _propietarios;
    private readonly IMascotaRepositorio _mascotas;
    private readonly IMapper _mapper;

    public PropietarioServicio(
        IPropietarioRepositorio propietarios,
        IMascotaRepositorio mascotas,
        IMapper mapper)
    {
        _propietarios = propietarios;
        _mascotas = mascotas;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PropietarioDto>> ObtenerTodosAsync(string? nombre, bool? conMascotas)
    {
        var lista = await _propietarios.ObtenerTodosAsync(nombre, conMascotas);
        return lista.Select(p => _mapper.Map<PropietarioDto>(p)).ToList();
    }

    public async Task<PropietarioDto?> ObtenerPorIdAsync(int id)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        return propietario is null ? null : _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<IEnumerable<MascotaDto>?> ObtenerMascotasAsync(int idPropietario)
    {
        if (!await _propietarios.ExisteAsync(idPropietario))
        {
            return null;
        }

        var lista = await _mascotas.ObtenerPorPropietarioAsync(idPropietario);
        return lista.Select(m => _mapper.Map<MascotaDto>(m)).ToList();
    }

    public async Task<PropietarioDto> CrearAsync(PropietarioCrearDto dto)
    {
        // Regla de negocio 1: el documento del propietario no se puede repetir.
        var documento = dto.Documento.Trim();
        if (await _propietarios.ExisteDocumentoAsync(documento))
        {
            throw new ReglaNegocioException($"Ya existe un propietario con el documento {documento}.");
        }

        var propietario = _mapper.Map<Propietario>(dto);
        propietario.Documento = documento;

        await _propietarios.AgregarAsync(propietario);
        return _mapper.Map<PropietarioDto>(propietario);
    }

    public async Task<bool> ActualizarAsync(int id, PropietarioActualizarDto dto)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        if (propietario is null)
        {
            return false;
        }

        // Map(origen, destino): copia los datos del DTO sobre la entidad existente.
        _mapper.Map(dto, propietario);
        await _propietarios.ActualizarAsync(propietario);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var propietario = await _propietarios.ObtenerPorIdAsync(id);
        if (propietario is null)
        {
            return false;
        }

        // Regla de negocio 2: no se puede eliminar un propietario que todavia tiene mascotas.
        if (await _propietarios.TieneMascotasAsync(id))
        {
            throw new ReglaNegocioException(
                "No se puede eliminar el propietario porque tiene mascotas registradas. Elimine o traspase primero sus mascotas.");
        }

        await _propietarios.EliminarAsync(propietario);
        return true;
    }
}
