import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import {
  TipoVehiculo,
  TipoVehiculoActualizar,
  TipoVehiculoCrear
} from '../models/tipo-vehiculo.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class TipoVehiculoService {

  private readonly url =
    `${API_CONFIG.baseUrl}/TiposVehiculo`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodos():
    Observable<Respuesta<TipoVehiculo[]>> {

    return this.http.get<
      Respuesta<TipoVehiculo[]>
    >(this.url);
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<TipoVehiculo>> {

    return this.http.get<
      Respuesta<TipoVehiculo>
    >(`${this.url}/${id}`);
  }

  agregar(
    tipo: TipoVehiculoCrear
  ): Observable<Respuesta<TipoVehiculo>> {

    return this.http.post<
      Respuesta<TipoVehiculo>
    >(this.url, tipo);
  }

  actualizar(
    id: number,
    tipo: TipoVehiculoActualizar
  ): Observable<Respuesta<TipoVehiculo>> {

    return this.http.put<
      Respuesta<TipoVehiculo>
    >(`${this.url}/${id}`, tipo);
  }

  eliminar(
    id: number
  ): Observable<Respuesta<TipoVehiculo>> {

    return this.http.delete<
      Respuesta<TipoVehiculo>
    >(`${this.url}/${id}`);
  }
}