import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import { Respuesta } from '../models/respuesta.model';
import {
  Vehiculo,
  VehiculoActualizar,
  VehiculoCrear
} from '../models/vehiculo.model';

@Injectable({
  providedIn: 'root'
})
export class VehiculoService {

  private readonly url =
    `${API_CONFIG.baseUrl}/Vehiculos`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodos():
    Observable<Respuesta<Vehiculo[]>> {

    return this.http.get<
      Respuesta<Vehiculo[]>
    >(this.url);
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<Vehiculo>> {

    return this.http.get<
      Respuesta<Vehiculo>
    >(`${this.url}/${id}`);
  }

  agregar(
    vehiculo: VehiculoCrear
  ): Observable<Respuesta<Vehiculo>> {

    return this.http.post<
      Respuesta<Vehiculo>
    >(this.url, vehiculo);
  }

  actualizar(
    id: number,
    vehiculo: VehiculoActualizar
  ): Observable<Respuesta<Vehiculo>> {

    return this.http.put<
      Respuesta<Vehiculo>
    >(`${this.url}/${id}`, vehiculo);
  }

  eliminar(
    id: number
  ): Observable<Respuesta<boolean>> {

    return this.http.delete<
      Respuesta<boolean>
    >(`${this.url}/${id}`);
  }
}