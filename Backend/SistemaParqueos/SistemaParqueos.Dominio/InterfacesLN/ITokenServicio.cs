using SistemaParqueos.Dominio.Entidades;

namespace SistemaParqueos.Dominio.InterfacesLN;

public interface ITokenServicio
{
    string GenerarToken(
        Usuario usuario,
        DateTime fechaExpiracion);
}