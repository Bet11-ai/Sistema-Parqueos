export interface Cliente {
  clienteId: number;
  nombre: string;
  apellidos: string;
  cedula: string;
  telefono: string | null;
  correo: string | null;
  activo: boolean;
  creadoEn: string;
}

export interface ClienteCrear {
  nombre: string;
  apellidos: string;
  cedula: string;
  telefono: string | null;
  correo: string | null;
}

export interface ClienteActualizar {
  nombre: string;
  apellidos: string;
  cedula: string;
  telefono: string | null;
  correo: string | null;
  activo: boolean;
}