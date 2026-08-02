import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import {
  ActualizarTarifa,
  CrearTarifa,
  Tarifa
} from '../models/tarifa.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class TarifaService {

  private readonly url =
    `${API_CONFIG.baseUrl}/Tarifas`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodos():
    Observable<Respuesta<Tarifa[]>> {

    return this.http.get<
      Respuesta<Tarifa[]>
    >(this.url);
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<Tarifa>> {

    return this.http.get<
      Respuesta<Tarifa>
    >(`${this.url}/${id}`);
  }

  obtenerPorTipoVehiculo(
    tipoVehiculoId: number
  ): Observable<Respuesta<Tarifa[]>> {

    return this.http.get<
      Respuesta<Tarifa[]>
    >(
      `${this.url}/por-tipo-vehiculo/${tipoVehiculoId}`
    );
  }

  agregar(
    tarifa: CrearTarifa
  ): Observable<Respuesta<Tarifa>> {

    return this.http.post<
      Respuesta<Tarifa>
    >(this.url, tarifa);
  }

  actualizar(
    id: number,
    tarifa: ActualizarTarifa
  ): Observable<Respuesta<Tarifa>> {

    return this.http.put<
      Respuesta<Tarifa>
    >(`${this.url}/${id}`, tarifa);
  }

  eliminar(
    id: number
  ): Observable<Respuesta<Tarifa>> {

    return this.http.delete<
      Respuesta<Tarifa>
    >(`${this.url}/${id}`);
  }
}