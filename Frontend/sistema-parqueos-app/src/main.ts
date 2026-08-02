import {
  bootstrapApplication
} from '@angular/platform-browser';

import {
  RouteReuseStrategy,
  provideRouter,
  withPreloading,
  PreloadAllModules
} from '@angular/router';

import {
  provideHttpClient,
  withInterceptors
} from '@angular/common/http';

import {
  IonicRouteStrategy,
  provideIonicAngular
} from '@ionic/angular/standalone';

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes';
import {
  authInterceptor
} from './app/interceptors/auth.interceptor';

bootstrapApplication(
  AppComponent,
  {
    providers: [
      {
        provide: RouteReuseStrategy,
        useClass: IonicRouteStrategy
      },

      provideIonicAngular(),

      provideRouter(
        routes,
        withPreloading(
          PreloadAllModules
        )
      ),

      provideHttpClient(
        withInterceptors([
          authInterceptor
        ])
      )
    ]
  }
).catch(error => {
  console.error(
    'No fue posible iniciar la aplicación:',
    error
  );
});