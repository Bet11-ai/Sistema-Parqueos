export interface EspacioParqueo {
  espacioId: number;
  parqueoId: number;
  nombreParqueo: string;
  numeroEspacio: string;
  disponible: boolean;
  activo: boolean;
  creadoEn: string;
}

export interface EspacioParqueoCrear {
  parqueoId: number;
  numeroEspacio: string;
  disponible: boolean;
}

export interface EspacioParqueoActualizar {
  parqueoId: number;
  numeroEspacio: string;
  disponible: boolean;
  activo: boolean;
}