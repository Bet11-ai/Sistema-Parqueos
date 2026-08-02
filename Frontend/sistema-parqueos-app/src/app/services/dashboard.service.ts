import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';
import { DashboardResumen } from '../models/dashboard.model';
import { Respuesta } from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {

  private readonly url =
    `${API_CONFIG.baseUrl}/Dashboard`;

  constructor(
    private readonly http: HttpClient
  ) {}

  obtenerResumen():
    Observable<Respuesta<DashboardResumen>> {

    return this.http.get<
      Respuesta<DashboardResumen>
    >(`${this.url}/resumen`);
  }
}