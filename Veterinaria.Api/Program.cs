using Microsoft.EntityFrameworkCore;
using Veterinaria.Datos.Models;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Negocio.Mapeos;
using Veterinaria.Negocio.Servicios;

var builder = WebApplication.CreateBuilder(args);

// La cadena de conexion NO va en el repositorio: se guarda en user-secrets (ver README).
var cadenaConexion = builder.Configuration.GetConnectionString("Veterinaria")
    ?? throw new InvalidOperationException(
        "Falta la cadena de conexion 'ConnectionStrings:Veterinaria'. " +
        "Configurala con: dotnet user-secrets set \"ConnectionStrings:Veterinaria\" \"Host=localhost;Port=5432;Database=veterinaria;Username=postgres;Password=TU_CLAVE\" --project Veterinaria.Api");

// Controladores (no minimal API)
builder.Services.AddControllers();

// OpenAPI + Swagger UI
builder.Services.AddOpenApi();

// DbContext (PostgreSQL)
builder.Services.AddDbContext<VeterinariaContext>(opciones => opciones.UseNpgsql(cadenaConexion));

// Repositorios
builder.Services.AddScoped<IPropietarioRepositorio, PropietarioRepositorio>();
builder.Services.AddScoped<IMascotaRepositorio, MascotaRepositorio>();

// Servicios
builder.Services.AddScoped<IPropietarioServicio, PropietarioServicio>();
builder.Services.AddScoped<IMascotaServicio, MascotaServicio>();

// AutoMapper (un unico MappingProfile en la capa de Negocio)
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opciones =>
    {
        opciones.SwaggerEndpoint("/openapi/v1.json", "Veterinaria API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
