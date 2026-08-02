import { Routes } from '@angular/router';

import { authGuard } from './guards/auth.guard';
import { loginGuard } from './guards/login.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'login',
    pathMatch: 'full'
  },
  {
    path: 'login',
    canActivate: [loginGuard],
    loadComponent: () =>
      import('./pages/login/login.page')
        .then(modulo => modulo.LoginPage)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/dashboard/dashboard.page')
        .then(modulo => modulo.DashboardPage)
  },
  {
    path: 'clientes',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/clientes/clientes.page')
        .then(modulo => modulo.ClientesPage)
  },
  {
    path: 'vehiculos',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/vehiculos/vehiculos.page')
        .then(modulo => modulo.VehiculosPage)
  },
  {
    path: 'parqueos',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/parqueos/parqueos.page')
        .then(modulo => modulo.ParqueosPage)
  },
  {
    path: 'tarifas',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/tarifas/tarifas.page')
        .then(modulo => modulo.TarifasPage)
  },
  {
    path: 'ingresos',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/ingresos/ingresos.page')
        .then(modulo => modulo.IngresosPage)
  },
  {
    path: 'facturas',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./pages/facturas/facturas.page')
        .then(modulo => modulo.FacturasPage)
  },
  {
    path: '**',
    redirectTo: 'login'
  }
];