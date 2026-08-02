import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import {
  Parqueo,
  ParqueoActualizar,
  ParqueoCrear
} from '../models/parqueo.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class ParqueoService {

  private readonly url =
    `${API_CONFIG.baseUrl}/Parqueos`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodos():
    Observable<Respuesta<Parqueo[]>> {

    return this.http.get<
      Respuesta<Parqueo[]>
    >(this.url);
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<Parqueo>> {

    return this.http.get<
      Respuesta<Parqueo>
    >(`${this.url}/${id}`);
  }

  agregar(
    parqueo: ParqueoCrear
  ): Observable<Respuesta<Parqueo>> {

    return this.http.post<
      Respuesta<Parqueo>
    >(this.url, parqueo);
  }

  actualizar(
    id: number,
    parqueo: ParqueoActualizar
  ): Observable<Respuesta<Parqueo>> {

    return this.http.put<
      Respuesta<Parqueo>
    >(`${this.url}/${id}`, parqueo);
  }

  eliminar(
    id: number
  ): Observable<Respuesta<Parqueo>> {

    return this.http.delete<
      Respuesta<Parqueo>
    >(`${this.url}/${id}`);
  }
}