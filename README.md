# Veterinaria API - Laboratorio S13 (API en capas con DTOs y AutoMapper)

Web API REST en capas con ASP.NET Core (.NET 10), Entity Framework Core 10 y PostgreSQL.
La API solo expone DTOs; la conversion entidad <-> DTO se hace con AutoMapper.

## 1. Tema

**Problema que resuelve:** una clinica veterinaria necesita llevar el registro de sus clientes (propietarios) y de las
mascotas que atiende, evitando duplicados y datos inconsistentes.

**Entidades y relacion 1:N**

| Entidad | Descripcion |
|---|---|
| `Propietario` (1) | Cliente de la veterinaria: nombres, apellidos, documento (unico), telefono, email, direccion, fecha de registro |
| `Mascota` (N) | Animal atendido: nombre, especie, raza, sexo (M/H), fecha de nacimiento, peso, microchip (unico), activo |

Un propietario tiene muchas mascotas; cada mascota pertenece a un solo propietario
(`mascota.id_propietario` -> `propietario.id_propietario`, propiedades de navegacion `Propietario.Mascotas` y
`Mascota.IdPropietarioNavigation`).

**Reglas de negocio** (capa Negocio, lanzan `ReglaNegocioException` -> el controlador responde 409 Conflict):

1. No se puede registrar un propietario con un documento que ya existe.
2. No se puede eliminar un propietario que todavia tiene mascotas.
3. Una mascota debe pertenecer a un propietario existente.
4. La fecha de nacimiento de una mascota no puede ser futura.
5. El microchip de una mascota (si se informa) no puede repetirse.

## 2. Estructura de la solucion

```
Veterinaria.slnx
db/veterinaria.sql                 Script PostgreSQL (tablas + datos de ejemplo)
Veterinaria.Entidades/             Entidades (Propietario, Mascota) y DTOs
Veterinaria.Datos/                 VeterinariaContext y repositorios (con interfaz)
Veterinaria.Negocio/               Servicios (con interfaz), reglas, MappingProfile, ReglaNegocioException
Veterinaria.Api/                   Controladores, Program.cs, Swagger, Veterinaria.Api.http
```

Referencias en cadena: **Api -> Negocio -> Datos -> Entidades**. Los controladores solo usan servicios (por su interfaz) y DTOs.

## 3. Puesta en marcha

```powershell
# 1) Crear la base de datos y los datos de ejemplo
& "C:\Program Files\PostgreSQL\18\bin\psql.exe" -U postgres -h localhost -f db/veterinaria.sql

# 2) Guardar la cadena de conexion fuera del repositorio (user-secrets)
dotnet user-secrets set "ConnectionStrings:Veterinaria" "Host=localhost;Port=5432;Database=veterinaria;Username=postgres;Password=TU_CLAVE" --project Veterinaria.Api

# 3) Restaurar, compilar y ejecutar
dotnet build
dotnet run --project Veterinaria.Api --launch-profile http
```

Swagger UI: http://localhost:5080/swagger (el documento OpenAPI esta en `/openapi/v1.json`).
Las pruebas de cada endpoint y de cada codigo (200, 201, 204, 400, 404, 409) estan en `Veterinaria.Api/Veterinaria.Api.http`.

### Scaffold (base de datos primero)

```powershell
dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=veterinaria;Username=postgres;Password=TU_CLAVE" Npgsql.EntityFrameworkCore.PostgreSQL --project Veterinaria.Datos --startup-project Veterinaria.Api --output-dir Models --context VeterinariaContext --no-onconfiguring --force
```

Despues se mueven `Propietario.cs` y `Mascota.cs` de `Veterinaria.Datos/Models` a `Veterinaria.Entidades`, se cambia su
namespace a `Veterinaria.Entidades` y se agrega `using Veterinaria.Entidades;` en `VeterinariaContext.cs`.

## 4. Endpoints (11)

| Metodo | Ruta | Respuestas |
|---|---|---|
| GET | `api/propietarios?nombre=&conMascotas=` | 200 |
| GET | `api/propietarios/{id}` | 200, 404 |
| GET | `api/propietarios/{id}/mascotas` (ruta anidada) | 200, 404 |
| POST | `api/propietarios` | 201, 400, 409 |
| PUT | `api/propietarios/{id}` | 204, 400, 404 |
| DELETE | `api/propietarios/{id}` | 204, 404, 409 |
| GET | `api/mascotas?especie=&nombre=&idPropietario=&activo=` | 200 |
| GET | `api/mascotas/{id}` | 200, 404 |
| POST | `api/mascotas` | 201, 400, 409 |
| PUT | `api/mascotas/{id}` | 204, 400, 404, 409 |
| DELETE | `api/mascotas/{id}` | 204, 404 |

## 5. Sustentacion de los mapeos (MappingProfile)

| Mapeo | Origen -> Destino | ¿Por que en ese sentido? | Configuracion especial |
|---|---|---|---|
| `Propietario` -> `PropietarioDto` | Entidad -> DTO de lectura | Es lo que la API devuelve al cliente: la entidad no debe salir de la API | `ForMember`: `CantidadMascotas` toma `Mascotas.Count` (dato calculado que no existe en la entidad) |
| `PropietarioCrearDto` -> `Propietario` | DTO de entrada -> Entidad | Es lo que llega del cliente y se convierte en entidad para guardarla; no se devuelve | `Ignore` en `IdPropietario`, `FechaRegistro` (la genera la BD) y `Mascotas` |
| `PropietarioActualizarDto` -> `Propietario` | DTO de entrada -> Entidad existente | Se usa con `Map(origen, destino)` para copiar los cambios sobre la entidad ya cargada | `Ignore` en `IdPropietario`, `Documento` (no se modifica), `FechaRegistro` y `Mascotas` |
| `Mascota` -> `MascotaDto` | Entidad -> DTO de lectura | Es lo que la API devuelve al cliente: la entidad no debe salir de la API | `ForMember` con `MapFrom`: `PropietarioNombre` = nombres + apellidos del propietario relacionado (en lugar del objeto) |
| `MascotaCrearDto` -> `Mascota` | DTO de entrada -> Entidad | Es lo que llega del cliente y se convierte en entidad para guardarla; no se devuelve | `ForMember` con `MapFrom(_ => true)`: `Activo` (toda mascota nueva nace activa); `Ignore` en `IdMascota` e `IdPropietarioNavigation` |
| `MascotaActualizarDto` -> `Mascota` | DTO de entrada -> Entidad existente | Se usa con `Map(origen, destino)` para copiar los cambios sobre la entidad ya cargada | `Ignore` en `IdMascota` e `IdPropietarioNavigation` |

**ReverseMap:** no se usa. Cada sentido tiene reglas distintas (la lectura calcula campos, la escritura ignora claves y
campos generados), asi que un mapeo inverso automatico no se puede justificar.

## 6. Observaciones y conclusiones (borrador)

- Separar la solucion en cuatro capas deja las responsabilidades claras: el controlador solo traduce HTTP, el servicio
  concentra las reglas de negocio y el repositorio es el unico que habla con EF Core.
- Los DTOs evitan exponer las entidades; al no devolver `Propietario.Mascotas` y `Mascota.IdPropietarioNavigation`
  desaparece el ciclo de JSON sin necesidad de `[JsonIgnore]`.
- AutoMapper reduce el codigo repetitivo; los `ForMember` se reservan para lo que no coincide por nombre
  (campos calculados, valores por defecto, campos ignorados).
- `ReglaNegocioException` permite que el servicio avise de una regla incumplida sin conocer HTTP, y el controlador la
  convierte en 409 Conflict.
- PostgreSQL funciona igual de bien que SQL Server con EF Core: solo cambian el proveedor (Npgsql), la cadena de conexion
  y la sintaxis del script SQL.
