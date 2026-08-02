export interface Vehiculo {
  vehiculoId: number;
  clienteId: number;
  nombreCliente: string;
  tipoVehiculoId: number;
  tipoVehiculo: string;
  placa: string;
  marca: string;
  modelo: string | null;
  color: string | null;
  activo: boolean;
  creadoEn: string;
}

export interface VehiculoCrear {
  clienteId: number;
  tipoVehiculoId: number;
  placa: string;
  marca: string;
  modelo: string | null;
  color: string | null;
}

export interface VehiculoActualizar {
  clienteId: number;
  tipoVehiculoId: number;
  placa: string;
  marca: string;
  modelo: string | null;
  color: string | null;
  activo: boolean;
}