export interface LoginSolicitud {
  correo: string;
  contrasena: string;
}

export interface UsuarioAutenticado {
  usuarioId: number;
  nombreCompleto: string;
  correo: string;
  rolId: number;
  rol: string;
  token: string;
  expiraEn: string;
}