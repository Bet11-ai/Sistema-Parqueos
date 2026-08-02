import {
  HttpInterceptorFn
} from '@angular/common/http';
import { inject } from '@angular/core';
import { from, switchMap } from 'rxjs';

import { AuthService } from '../services/auth.service';

export const authInterceptor:
  HttpInterceptorFn = (request, next) => {

    const authService =
      inject(AuthService);

    return from(
      authService.obtenerToken()
    ).pipe(
      switchMap(token => {
        if (!token) {
          return next(request);
        }

        const solicitudAutorizada =
          request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
            }
          });

        return next(
          solicitudAutorizada
        );
      })
    );
  };