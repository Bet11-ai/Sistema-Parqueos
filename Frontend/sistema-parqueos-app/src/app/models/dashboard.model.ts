export interface ActividadReciente {
  ingresoId: number;
  placa: string;
  cliente: string;
  parqueo: string;
  numeroEspacio: string;
  fechaIngreso: string;
  fechaSalida: string | null;
  estado: string;
}

export interface DashboardResumen {
  vehiculosDentro: number;
  espaciosDisponibles: number;
  espaciosOcupados: number;
  totalEspaciosActivos: number;
  ingresosHoy: number;
  facturacionHoy: number;
  facturacionMes: number;
  clientesActivos: number;
  vehiculosActivos: number;
  porcentajeOcupacion: number;
  actividadReciente: ActividadReciente[];
}