export interface IngresoVehiculo {
  ingresoId: number;
  vehiculoId: number;
  placa: string;
  espacioId: number;
  numeroEspacio: string;
  nombreParqueo: string;
  fechaIngreso: string;
  fechaSalida: string | null;
  estado: string;
}

export interface IngresoVehiculoCrear {
  vehiculoId: number;
  espacioId: number;
}

export interface SalidaVehiculoRespuesta {
  ingresoId: number;
  placa: string;
  numeroEspacio: string;
  fechaIngreso: string;
  fechaSalida: string;
  horasCobradas: number;
  montoHora: number;
  montoTotal: number;
  facturaId: number;
  estado: string;
}
