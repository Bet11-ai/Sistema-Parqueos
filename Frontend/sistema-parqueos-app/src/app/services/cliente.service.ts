import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import {
  Cliente,
  ClienteActualizar,
  ClienteCrear
} from '../models/cliente.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {

  private readonly url = `${API_CONFIG.baseUrl}/Clientes`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodos(): Observable<Respuesta<Cliente[]>> {
    return this.http.get<Respuesta<Cliente[]>>(
      this.url
    );
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<Cliente>> {
    return this.http.get<Respuesta<Cliente>>(
      `${this.url}/${id}`
    );
  }

  agregar(
    cliente: ClienteCrear
  ): Observable<Respuesta<Cliente>> {
    return this.http.post<Respuesta<Cliente>>(
      this.url,
      cliente
    );
  }

  actualizar(
    id: number,
    cliente: ClienteActualizar
  ): Observable<Respuesta<Cliente>> {
    return this.http.put<Respuesta<Cliente>>(
      `${this.url}/${id}`,
      cliente
    );
  }

  eliminar(
    id: number
  ): Observable<Respuesta<boolean>> {
    return this.http.delete<Respuesta<boolean>>(
      `${this.url}/${id}`
    );
  }
}