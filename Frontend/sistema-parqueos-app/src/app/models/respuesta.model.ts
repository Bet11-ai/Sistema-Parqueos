export interface Respuesta<T = unknown> {
  exito: boolean;
  mensaje: string;
  valorRetorno: T | null;
  errores: string[] | null;
  codigoEstado: number;
}