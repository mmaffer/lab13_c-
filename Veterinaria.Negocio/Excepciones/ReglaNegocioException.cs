namespace Veterinaria.Negocio.Excepciones;

/// <summary>
/// Se lanza desde la capa de Negocio cuando se incumple una regla de negocio.
/// El controlador la convierte en una respuesta 409 Conflict.
/// </summary>
public class ReglaNegocioException : Exception
{
    public ReglaNegocioException(string mensaje) : base(mensaje)
    {
    }
}
