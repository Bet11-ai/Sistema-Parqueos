export interface Factura {
  facturaId: number;
  ingresoId: number;
  placa: string;
  tipoVehiculo: string;
  numeroEspacio: string;
  nombreParqueo: string;
  fechaIngreso: string;
  fechaSalida: string | null;
  fechaFactura: string;
  horasCobradas: number;
  montoTotal: number;
}