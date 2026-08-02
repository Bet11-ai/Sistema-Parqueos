using Microsoft.AspNetCore.Identity;
using SistemaParqueos.Dominio.DTO.Auth;
using SistemaParqueos.Dominio.Entidades;
using SistemaParqueos.Dominio.InterfacesAD;
using SistemaParqueos.Dominio.InterfacesLN;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.LogicaNegocio.Implementaciones;

public class AuthLN : IAuthLN
{
    private readonly IUnidadTrabajoEF _unidadTrabajo;
    private readonly IPasswordHasher<Usuario> _passwordHasher;
    private readonly ITokenServicio _tokenServicio;

    public AuthLN(
        IUnidadTrabajoEF unidadTrabajo,
        IPasswordHasher<Usuario> passwordHasher,
        ITokenServicio tokenServicio)
    {
        _unidadTrabajo = unidadTrabajo;
        _passwordHasher = passwordHasher;
        _tokenServicio = tokenServicio;
    }

    public async Task<Respuesta>
        CrearPrimerAdministradorAsync(
            CrearPrimerAdministradorDto dto)
    {
        try
        {
            var cantidadUsuarios =
                await _unidadTrabajo.Usuarios
                    .ContarAsync(usuario => true);

            if (cantidadUsuarios > 0)
            {
                return Respuesta.Fallida(
                    "El primer administrador ya fue creado.",
                    409);
            }

            var rolAdministrador =
                await _unidadTrabajo.Roles
                    .ObtenerEntidadAsync(rol =>
                        rol.Nombre == "Administrador" &&
                        rol.Activo);

            if (rolAdministrador is null)
            {
                return Respuesta.Fallida(
                    "No existe el rol Administrador.",
                    409);
            }

            NormalizarCorreo(dto);

            var usuario = new Usuario
            {
                RolId = rolAdministrador.RolId,
                NombreCompleto =
                    dto.NombreCompleto.Trim(),
                Correo = dto.Correo,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "CONFIGURACION_INICIAL"
            };

            usuario.ContrasenaHash =
                _passwordHasher.HashPassword(
                    usuario,
                    dto.Contrasena);

            await _unidadTrabajo.Usuarios
                .InsertarAsync(usuario);

            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                new
                {
                    usuario.UsuarioId,
                    usuario.NombreCompleto,
                    usuario.Correo,
                    Rol = rolAdministrador.Nombre
                },
                "Primer administrador creado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> RegistrarUsuarioAsync(
        RegistrarUsuarioDto dto)
    {
        try
        {
            NormalizarCorreo(dto);

            var correoExiste =
                await _unidadTrabajo.Usuarios
                    .ObtenerEntidadAsync(usuario =>
                        usuario.Correo == dto.Correo);

            if (correoExiste is not null)
            {
                return Respuesta.Fallida(
                    "Ya existe un usuario con ese correo.",
                    409);
            }

            var rol =
                await _unidadTrabajo.Roles
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.RolId == dto.RolId);

            if (rol is null)
            {
                return Respuesta.Fallida(
                    "El rol seleccionado no existe.",
                    400);
            }

            if (!rol.Activo)
            {
                return Respuesta.Fallida(
                    "El rol seleccionado está inactivo.",
                    409);
            }

            var usuario = new Usuario
            {
                RolId = dto.RolId,
                NombreCompleto =
                    dto.NombreCompleto.Trim(),
                Correo = dto.Correo,
                Activo = true,
                CreadoEn = DateTime.UtcNow,
                CreadoPor = "ADMINISTRADOR"
            };

            usuario.ContrasenaHash =
                _passwordHasher.HashPassword(
                    usuario,
                    dto.Contrasena);

            await _unidadTrabajo.Usuarios
                .InsertarAsync(usuario);

            await _unidadTrabajo.CompletarAsync();

            return Respuesta.Correcta(
                new
                {
                    usuario.UsuarioId,
                    usuario.NombreCompleto,
                    usuario.Correo,
                    usuario.RolId,
                    Rol = rol.Nombre
                },
                "Usuario registrado correctamente.",
                201);
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> IniciarSesionAsync(
        LoginDto dto)
    {
        try
        {
            dto.Correo =
                dto.Correo.Trim().ToLowerInvariant();

            var usuario =
                await _unidadTrabajo.Usuarios
                    .ObtenerEntidadAsync(
                        registro =>
                            registro.Correo == dto.Correo,
                        registro => registro.Rol);

            if (usuario is null || !usuario.Activo)
            {
                return Respuesta.Fallida(
                    "Correo o contraseña incorrectos.",
                    401);
            }

            if (!usuario.Rol.Activo)
            {
                return Respuesta.Fallida(
                    "El rol del usuario está inactivo.",
                    403);
            }

            var resultadoVerificacion =
                _passwordHasher.VerifyHashedPassword(
                    usuario,
                    usuario.ContrasenaHash,
                    dto.Contrasena);

            if (resultadoVerificacion ==
                PasswordVerificationResult.Failed)
            {
                return Respuesta.Fallida(
                    "Correo o contraseña incorrectos.",
                    401);
            }

            var fechaExpiracion =
                DateTime.UtcNow.AddHours(8);

            var token =
                _tokenServicio.GenerarToken(
                    usuario,
                    fechaExpiracion);

            var resultado = new AuthRespuestaDto
            {
                UsuarioId = usuario.UsuarioId,
                NombreCompleto =
                    usuario.NombreCompleto,
                Correo = usuario.Correo,
                RolId = usuario.RolId,
                Rol = usuario.Rol.Nombre,
                Token = token,
                ExpiraEn = fechaExpiracion
            };

            return Respuesta.Correcta(
                resultado,
                "Inicio de sesión correcto.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    public async Task<Respuesta> ObtenerRolesAsync()
    {
        try
        {
            var roles =
                await _unidadTrabajo.Roles
                    .BuscarAsync(rol => rol.Activo);

            var resultado = roles
                .OrderBy(rol => rol.Nombre)
                .Select(rol => new
                {
                    rol.RolId,
                    rol.Nombre,
                    rol.Descripcion
                })
                .ToList();

            return Respuesta.Correcta(
                resultado,
                "Roles consultados correctamente.");
        }
        catch (Exception ex)
        {
            return CrearRespuestaError(ex);
        }
    }

    private static void NormalizarCorreo(
        CrearPrimerAdministradorDto dto)
    {
        dto.Correo =
            dto.Correo.Trim().ToLowerInvariant();
    }

    private static void NormalizarCorreo(
        RegistrarUsuarioDto dto)
    {
        dto.Correo =
            dto.Correo.Trim().ToLowerInvariant();
    }

    private static Respuesta CrearRespuestaError(
        Exception ex)
    {
        return Respuesta.Fallida(
            "Ocurrió un error al procesar la autenticación.",
            500,
            new List<string>
            {
                ex.Message
            });
    }
}