import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import {
  EspacioParqueo,
  EspacioParqueoActualizar,
  EspacioParqueoCrear
} from '../models/espacio-parqueo.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class EspacioParqueoService {

  private readonly url =
    `${API_CONFIG.baseUrl}/EspaciosParqueo`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodos():
    Observable<Respuesta<EspacioParqueo[]>> {

    return this.http.get<
      Respuesta<EspacioParqueo[]>
    >(this.url);
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<EspacioParqueo>> {

    return this.http.get<
      Respuesta<EspacioParqueo>
    >(`${this.url}/${id}`);
  }

  obtenerPorParqueo(
    parqueoId: number
  ): Observable<Respuesta<EspacioParqueo[]>> {

    return this.http.get<
      Respuesta<EspacioParqueo[]>
    >(
      `${this.url}/por-parqueo/${parqueoId}`
    );
  }

  agregar(
    espacio: EspacioParqueoCrear
  ): Observable<Respuesta<EspacioParqueo>> {

    return this.http.post<
      Respuesta<EspacioParqueo>
    >(this.url, espacio);
  }

  actualizar(
    id: number,
    espacio: EspacioParqueoActualizar
  ): Observable<Respuesta<EspacioParqueo>> {

    return this.http.put<
      Respuesta<EspacioParqueo>
    >(`${this.url}/${id}`, espacio);
  }

  eliminar(
    id: number
  ): Observable<Respuesta<EspacioParqueo>> {

    return this.http.delete<
      Respuesta<EspacioParqueo>
    >(`${this.url}/${id}`);
  }
}