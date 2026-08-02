import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Preferences } from '@capacitor/preferences';

import {
  Observable,
  from,
  switchMap
} from 'rxjs';

import { API_CONFIG } from '../constants/api.constants';

import {
  LoginSolicitud,
  UsuarioAutenticado
} from '../models/auth.model';

import {
  Respuesta
} from '../models/respuesta.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly url =
    `${API_CONFIG.baseUrl}/Auth`;

  private readonly tokenKey =
    'sistema_parqueos_token';

  private readonly usuarioKey =
    'sistema_parqueos_usuario';

  constructor(
    private readonly http: HttpClient,
    private readonly router: Router
  ) {}

  iniciarSesion(
    credenciales: LoginSolicitud
  ): Observable<Respuesta<UsuarioAutenticado>> {

    return this.http
      .post<Respuesta<UsuarioAutenticado>>(
        `${this.url}/login`,
        credenciales
      )
      .pipe(
        switchMap(respuesta => {

          if (
            !respuesta.exito ||
            !respuesta.valorRetorno
          ) {
            return from(
              Promise.resolve(respuesta)
            );
          }

          return from(
            this.guardarSesion(
              respuesta.valorRetorno
            ).then(() => respuesta)
          );
        })
      );
  }

  private async guardarSesion(
    usuario: UsuarioAutenticado
  ): Promise<void> {

    await Preferences.set({
      key: this.tokenKey,
      value: usuario.token
    });

    await Preferences.set({
      key: this.usuarioKey,
      value: JSON.stringify(usuario)
    });
  }

  async obtenerToken():
    Promise<string | null> {

    const resultado =
      await Preferences.get({
        key: this.tokenKey
      });

    return resultado.value;
  }

  async obtenerUsuario():
    Promise<UsuarioAutenticado | null> {

    const resultado =
      await Preferences.get({
        key: this.usuarioKey
      });

    if (!resultado.value) {
      return null;
    }

    try {
      return JSON.parse(
        resultado.value
      ) as UsuarioAutenticado;
    } catch {
      await this.limpiarSesion();
      return null;
    }
  }

  async estaAutenticado():
    Promise<boolean> {

    const token =
      await this.obtenerToken();

    const usuario =
      await this.obtenerUsuario();

    if (!token || !usuario) {
      await this.limpiarSesion();
      return false;
    }

    const fechaExpiracion =
      new Date(usuario.expiraEn);

    if (
      Number.isNaN(
        fechaExpiracion.getTime()
      ) ||
      fechaExpiracion <= new Date()
    ) {
      await this.limpiarSesion();
      return false;
    }

    return true;
  }

  async esAdministrador():
    Promise<boolean> {

    const usuario =
      await this.obtenerUsuario();

    return (
      usuario?.rol ===
      'Administrador'
    );
  }

  async cerrarSesion():
    Promise<void> {

    await this.limpiarSesion();

    await this.router.navigateByUrl(
      '/login',
      {
        replaceUrl: true
      }
    );
  }

  private async limpiarSesion():
    Promise<void> {

    await Preferences.remove({
      key: this.tokenKey
    });

    await Preferences.remove({
      key: this.usuarioKey
    });
  }
}