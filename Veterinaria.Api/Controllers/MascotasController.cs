using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MascotasController : ControllerBase
{
    private readonly IMascotaServicio _servicio;

    public MascotasController(IMascotaServicio servicio)
    {
        _servicio = servicio;
    }

    /// <summary>Lista mascotas. Filtros opcionales: especie, nombre (contiene), idPropietario y activo.</summary>
    [HttpGet]
    [ProducesResponseType<IEnumerable<MascotaDto>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MascotaDto>>> Listar(
        [FromQuery] string? especie,
        [FromQuery] string? nombre,
        [FromQuery] int? idPropietario,
        [FromQuery] bool? activo)
    {
        var mascotas = await _servicio.ObtenerTodasAsync(especie, nombre, idPropietario, activo);
        return Ok(mascotas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<MascotaDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MascotaDto>> ObtenerPorId(int id)
    {
        var mascota = await _servicio.ObtenerPorIdAsync(id);
        if (mascota is null)
        {
            return NotFound();
        }

        return Ok(mascota);
    }

    [HttpPost]
    [ProducesResponseType<MascotaDto>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<MascotaDto>> Crear(MascotaCrearDto dto)
    {
        try
        {
            var creada = await _servicio.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.IdMascota }, creada);
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
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Actualizar(int id, MascotaActualizarDto dto)
    {
        try
        {
            var existe = await _servicio.ActualizarAsync(id, dto);
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

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var existe = await _servicio.EliminarAsync(id);
        if (!existe)
        {
            return NotFound();
        }

        return NoContent();
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
