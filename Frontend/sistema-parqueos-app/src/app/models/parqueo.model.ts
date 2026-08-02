export interface Parqueo {
  parqueoId: number;
  nombreParqueo: string;
  direccion: string;
  telefono: string | null;
  capacidadTotal: number;
  activo: boolean;
  creadoEn: string;
}

export interface ParqueoCrear {
  nombreParqueo: string;
  direccion: string;
  telefono: string | null;
  capacidadTotal: number;
}

export interface ParqueoActualizar {
  nombreParqueo: string;
  direccion: string;
  telefono: string | null;
  capacidadTotal: number;
  activo: boolean;
}