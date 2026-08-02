import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import {
  IngresoVehiculo,
  IngresoVehiculoCrear,
  SalidaVehiculoRespuesta
} from '../models/ingreso-vehiculo.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({ providedIn: 'root' })
export class IngresoVehiculoService {
  private readonly url = `${API_CONFIG.baseUrl}/IngresosVehiculo`;

  constructor(private readonly http: HttpClient) {}

  obtenerTodos(): Observable<Respuesta<IngresoVehiculo[]>> {
    return this.http.get<Respuesta<IngresoVehiculo[]>>(this.url);
  }

  obtenerActivos(): Observable<Respuesta<IngresoVehiculo[]>> {
    return this.http.get<Respuesta<IngresoVehiculo[]>>(`${this.url}/activos`);
  }

  obtenerPorId(id: number): Observable<Respuesta<IngresoVehiculo>> {
    return this.http.get<Respuesta<IngresoVehiculo>>(`${this.url}/${id}`);
  }

  agregar(ingreso: IngresoVehiculoCrear): Observable<Respuesta<IngresoVehiculo>> {
    return this.http.post<Respuesta<IngresoVehiculo>>(this.url, ingreso);
  }

  registrarSalida(ingresoId: number): Observable<Respuesta<SalidaVehiculoRespuesta>> {
    return this.http.put<Respuesta<SalidaVehiculoRespuesta>>(
      `${this.url}/${ingresoId}/salida`,
      {}
    );
  }
}
