using AutoMapper;
using Veterinaria.Entidades;
using Veterinaria.Entidades.Dtos;

namespace Veterinaria.Negocio.Mapeos;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // ---------- Propietario ----------
        // Entidad -> DTO de lectura (lo que la API devuelve)
        CreateMap<Propietario, PropietarioDto>()
            .ForMember(d => d.CantidadMascotas, o => o.MapFrom(s => s.Mascotas.Count));

        // DTO de entrada -> Entidad (crear)
        CreateMap<PropietarioCrearDto, Propietario>()
            .ForMember(d => d.IdPropietario, o => o.Ignore())
            .ForMember(d => d.FechaRegistro, o => o.Ignore())
            .ForMember(d => d.Mascotas, o => o.Ignore());

        // DTO de entrada -> Entidad existente (actualizar con Map(origen, destino))
        CreateMap<PropietarioActualizarDto, Propietario>()
            .ForMember(d => d.IdPropietario, o => o.Ignore())
            .ForMember(d => d.Documento, o => o.Ignore())
            .ForMember(d => d.FechaRegistro, o => o.Ignore())
            .ForMember(d => d.Mascotas, o => o.Ignore());

        // ---------- Mascota ----------
        // Entidad -> DTO de lectura (devuelve el nombre del propietario, no el objeto)
        CreateMap<Mascota, MascotaDto>()
            .ForMember(d => d.PropietarioNombre,
                o => o.MapFrom(s => s.IdPropietarioNavigation.Nombres + " " + s.IdPropietarioNavigation.Apellidos));

        // DTO de entrada -> Entidad (crear): toda mascota nueva nace activa
        CreateMap<MascotaCrearDto, Mascota>()
            .ForMember(d => d.IdMascota, o => o.Ignore())
            .ForMember(d => d.Activo, o => o.MapFrom(_ => true))
            .ForMember(d => d.IdPropietarioNavigation, o => o.Ignore());

        // DTO de entrada -> Entidad existente (actualizar con Map(origen, destino))
        CreateMap<MascotaActualizarDto, Mascota>()
            .ForMember(d => d.IdMascota, o => o.Ignore())
            .ForMember(d => d.IdPropietarioNavigation, o => o.Ignore());
    }
}
