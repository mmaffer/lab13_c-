using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropietariosController : ControllerBase
{
    private readonly IPropietarioServicio _servicio;

    public PropietariosController(IPropietarioServicio servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista propietarios. Filtros opcionales: nombre (contiene) y conMascotas (true/false).</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<PropietarioDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PropietarioDto>>> Listar(
        [FromQuery] string? nombre, [FromQuery] bool? conMascotas)
    {
        var propietarios = await _servicio.ObtenerTodosAsync(nombre, conMascotas);
        return Ok(propietarios);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<PropietarioDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PropietarioDto>> ObtenerPorId(int id)
    {
        var propietario = await _servicio.ObtenerPorIdAsync(id);
        if (propietario is null)
        {
            return NotFound();
        }

        return Ok(propietario);
    }

    /// <summary>Ruta anidada: mascotas de un propietario.</summary>
    [HttpGet("{id:int}/mascotas")]
    [ProducesResponseType<IEnumerable<MascotaDto>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> ListarMascotas(int id)
    {
        var mascotas = await _servicio.ObtenerMascotasAsync(id);
        if (mascotas is null)
        {
            return NotFound();
        }

        return Ok(mascotas);
    }

    [HttpPost]
    [ProducesResponseType<PropietarioDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<PropietarioDto>> Crear(PropietarioCrearDto dto)
    {
        try
        {
            var creado = await _servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.IdPropietario }, creado);
        }
        catch (ReglaNegocioException ex)
        {
            return ConflictoDeNegocio(ex);
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Actualizar(int id, PropietarioActualizarDto dto)
    {
        var existe = await _servicio.ActualizarAsync(id, dto);
        if (!existe)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Eliminar(int id)
    {
        try
        {
            var existe = await _servicio.EliminarAsync(id);
            if (!existe)
            {
                return NotFound();
            }

            return NoContent();
        }
        catch (ReglaNegocioException ex)
        {
            return ConflictoDeNegocio(ex);
        }
    }

    private ConflictObjectResult ConflictoDeNegocio(ReglaNegocioException ex)
    {
        return Conflict(new ProblemDetails
        {
            Title = "Regla de negocio incumplida",
            Detail = ex.Message,
            Status = StatusCodes.Status409Conflict
        });
    }
}
