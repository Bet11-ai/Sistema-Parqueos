using SistemaParqueos.Dominio.DTO.Auth;
using SistemaParqueos.Utilitarios;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface IAuthLN
{
    Task<Respuesta> CrearPrimerAdministradorAsync(
        CrearPrimerAdministradorDto dto);

    Task<Respuesta> RegistrarUsuarioAsync(
        RegistrarUsuarioDto dto);

    Task<Respuesta> IniciarSesionAsync(
        LoginDto dto);

    Task<Respuesta> ObtenerRolesAsync();
}