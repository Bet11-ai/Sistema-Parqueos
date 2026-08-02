using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SistemaParqueos.AccesoDatos.Contexto;
using SistemaParqueos.AccesoDatos.Implementaciones;
using SistemaParqueos.API.Middlewares;
using SistemaParqueos.API.Servicios;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.LogicaNegocio.Implementaciones;
using SistemaParqueos.Utilitarios;
using System.Text;

var builder = WebApplication.CreateBuilder(args);



var cadenaConexion =
    builder.Configuration.GetConnectionString(
        "ParqueosConnection");

if (string.IsNullOrWhiteSpace(cadenaConexion))
{
    throw new InvalidOperationException(
        "No se encontró la cadena de conexión ParqueosConnection.");
}

builder.Services.AddDbContext<ParqueosDbContext>(
    opciones =>
        opciones.UseSqlServer(cadenaConexion));



builder.Services.AddScoped<
    IUnidadTrabajoEF,
    UnidadTrabajoEF>();


builder.Services.AddScoped<
    IClienteLN,
    ClienteLN>();

builder.Services.AddScoped<
    IVehiculoLN,
    VehiculoLN>();

builder.Services.AddScoped<
    IParqueoLN,
    ParqueoLN>();

builder.Services.AddScoped<
    IEspacioParqueoLN,
    EspacioParqueoLN>();

builder.Services.AddScoped<
    ITarifaLN,
    TarifaLN>();

builder.Services.AddScoped<
    IIngresoVehiculoLN,
    IngresoVehiculoLN>();

builder.Services.AddScoped<
    IFacturaLN,
    FacturaLN>();


builder.Services.AddScoped<
    IPasswordHasher<Usuario>,
    PasswordHasher<Usuario>>();

builder.Services.AddScoped<
    ITokenServicio,
    JwtTokenServicio>();

builder.Services.AddScoped<
    IAuthLN,
    AuthLN>();

builder.Services.AddScoped<
    IDashboardLN,
    DashboardLN>();

builder.Services.AddScoped<
    ITipoVehiculoLN,
    TipoVehiculoLN>();

var claveJwt =
    builder.Configuration["Jwt:Clave"];

var emisorJwt =
    builder.Configuration["Jwt:Emisor"];

var audienciaJwt =
    builder.Configuration["Jwt:Audiencia"];

if (string.IsNullOrWhiteSpace(claveJwt))
{
    throw new InvalidOperationException(
        "No se encontró la configuración Jwt:Clave.");
}

if (string.IsNullOrWhiteSpace(emisorJwt))
{
    throw new InvalidOperationException(
        "No se encontró la configuración Jwt:Emisor.");
}

if (string.IsNullOrWhiteSpace(audienciaJwt))
{
    throw new InvalidOperationException(
        "No se encontró la configuración Jwt:Audiencia.");
}

builder.Services
    .AddAuthentication(
        opciones =>
        {
            opciones.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            opciones.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
    .AddJwtBearer(
        opciones =>
        {
            opciones.RequireHttpsMetadata = true;
            opciones.SaveToken = true;

            opciones.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = emisorJwt,
                    ValidAudience = audienciaJwt,

                    IssuerSigningKey =
                        new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                claveJwt)),

                    ClockSkew = TimeSpan.Zero
                };

            opciones.Events =
                new JwtBearerEvents
                {
                    OnChallenge = async contexto =>
                    {
                        contexto.HandleResponse();

                        var respuesta =
                            Respuesta.Fallida(
                                "Debe iniciar sesión para acceder a este recurso.",
                                StatusCodes.Status401Unauthorized);

                        contexto.Response.StatusCode =
                            StatusCodes.Status401Unauthorized;

                        contexto.Response.ContentType =
                            "application/json; charset=utf-8";

                        await contexto.Response
                            .WriteAsJsonAsync(respuesta);
                    },

                    OnForbidden = async contexto =>
                    {
                        var respuesta =
                            Respuesta.Fallida(
                                "No tiene permisos para realizar esta operación.",
                                StatusCodes.Status403Forbidden);

                        contexto.Response.StatusCode =
                            StatusCodes.Status403Forbidden;

                        contexto.Response.ContentType =
                            "application/json; charset=utf-8";

                        await contexto.Response
                            .WriteAsJsonAsync(respuesta);
                    }
                };
        });

builder.Services.AddAuthorization();



builder.Services.AddControllers();



builder.Services.Configure<ApiBehaviorOptions>(
    opciones =>
    {
        opciones.InvalidModelStateResponseFactory =
            contexto =>
            {
                var errores =
                    contexto.ModelState
                        .Where(elemento =>
                            elemento.Value?.Errors.Count > 0)
                        .SelectMany(elemento =>
                            elemento.Value!.Errors)
                        .Select(error =>
                            string.IsNullOrWhiteSpace(
                                error.ErrorMessage)
                                ? "Uno de los datos enviados no es válido."
                                : error.ErrorMessage)
                        .Distinct()
                        .ToList();

                var respuesta =
                    Respuesta.Fallida(
                        "Existen errores de validación.",
                        StatusCodes.Status400BadRequest,
                        errores);

                return new BadRequestObjectResult(
                    respuesta);
            };
    });



builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(
    opciones =>
    {
        opciones.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title =
                    "Sistema de Gestión de Parqueos API",

                Version = "v1",

                Description =
                    "API para administrar clientes, vehículos, " +
                    "parqueos, espacios, tarifas, ingresos, " +
                    "facturas y autenticación de usuarios."
            });

        opciones.AddSecurityDefinition(
            "Bearer",
            new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,

                Description =
                    "Ingrese únicamente el token JWT. " +
                    "Swagger agregará automáticamente la palabra Bearer."
            });

        opciones.AddSecurityRequirement(
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference =
                            new OpenApiReference
                            {
                                Type =
                                    ReferenceType.SecurityScheme,

                                Id = "Bearer"
                            }
                    },
                    Array.Empty<string>()
                }
            });
    });



builder.Services.AddCors(
    opciones =>
    {
        opciones.AddPolicy(
            "PoliticaIonic",
            politica =>
            {
                politica
                    .AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
    });

var app = builder.Build();



app.UseMiddleware<
    ManejadorExcepcionesMiddleware>();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(
        opciones =>
        {
            opciones.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "Sistema de Parqueos API v1");

            opciones.DocumentTitle =
                "Sistema de Parqueos";
        });
}


app.UseHttpsRedirection();

app.UseCors("PoliticaIonic");


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();