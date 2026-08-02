export interface Tarifa {
  tarifaId: number;
  tipoVehiculoId: number;
  tipoVehiculo: string;
  descripcion: string;
  montoHora: number;
  activo: boolean;
  creadoEn: string;
}

export interface CrearTarifa {
  tipoVehiculoId: number;
  descripcion: string;
  montoHora: number;
}

export interface ActualizarTarifa {
  tipoVehiculoId: number;
  descripcion: string;
  montoHora: number;
  activo: boolean;
}