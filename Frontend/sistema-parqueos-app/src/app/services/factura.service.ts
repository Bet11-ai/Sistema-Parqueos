import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import { Factura } from '../models/factura.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class FacturaService {

  private readonly url =
    `${API_CONFIG.baseUrl}/Facturas`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerTodas():
    Observable<Respuesta<Factura[]>> {

    return this.http.get<
      Respuesta<Factura[]>
    >(this.url);
  }

  obtenerPorId(
    id: number
  ): Observable<Respuesta<Factura>> {

    return this.http.get<
      Respuesta<Factura>
    >(`${this.url}/${id}`);
  }

  obtenerPorIngreso(
    ingresoId: number
  ): Observable<Respuesta<Factura>> {

    return this.http.get<
      Respuesta<Factura>
    >(
      `${this.url}/por-ingreso/${ingresoId}`
    );
  }
}