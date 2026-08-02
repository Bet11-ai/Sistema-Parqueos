export interface TipoVehiculo {
  tipoVehiculoId: number;
  descripcion: string;
  activo: boolean;
  creadoEn: string;
}

export interface TipoVehiculoCrear {
  descripcion: string;
}

export interface TipoVehiculoActualizar {
  descripcion: string;
  activo: boolean;
}