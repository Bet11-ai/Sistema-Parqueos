import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import {
  NavigationEnd,
  Router,
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import {
  IonApp,
  IonAvatar,
  IonContent,
  IonIcon,
  IonItem,
  IonLabel,
  IonList,
  IonMenu,
  IonMenuToggle,
  IonRouterOutlet,
  IonSplitPane
} from '@ionic/angular/standalone';

import { filter } from 'rxjs';

import { addIcons } from 'ionicons';
import {
  barChartOutline,
  businessOutline,
  carSportOutline,
  documentTextOutline,
  gridOutline,
  logOutOutline,
  peopleOutline,
  personCircleOutline,
   pricetagOutline,
  settingsOutline,
  speedometerOutline
} from 'ionicons/icons';

import { UsuarioAutenticado } from './models/auth.model';
import { AuthService } from './services/auth.service';

interface OpcionMenu {
  titulo: string;
  icono: string;
  ruta: string;
  claseColor: string;
  soloAdministrador?: boolean;
}

@Component({
  selector: 'app-root',
  templateUrl: 'app.component.html',
  styleUrls: ['app.component.scss'],
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    IonApp,
    IonSplitPane,
    IonMenu,
    IonContent,
    IonList,
    IonItem,
    IonIcon,
    IonLabel,
    IonMenuToggle,
    IonRouterOutlet,
    IonAvatar
  ]
})
export class AppComponent implements OnInit {

  usuario: UsuarioAutenticado | null = null;
  esAdministrador = false;
  mostrarMenu = false;

  opcionesMenu: OpcionMenu[] = [
  {
    titulo: 'Dashboard',
    icono: 'speedometer-outline',
    ruta: '/dashboard',
    claseColor: 'color-lima'
  },
  {
    titulo: 'Clientes',
    icono: 'people-outline',
    ruta: '/clientes',
    claseColor: 'color-lima',
    soloAdministrador: true
  },
  {
    titulo: 'Vehículos',
    icono: 'car-sport-outline',
    ruta: '/vehiculos',
    claseColor: 'color-turquesa',
    soloAdministrador: true
  },
  {
    titulo: 'Parqueos',
    icono: 'business-outline',
    ruta: '/parqueos',
    claseColor: 'color-celeste',
    soloAdministrador: true
  },
  {
    titulo: 'Tarifas',
    icono: 'pricetag-outline',
    ruta: '/tarifas',
    claseColor: 'color-morado',
    soloAdministrador: true
  },
  {
    titulo: 'Ingresos',
    icono: 'grid-outline',
    ruta: '/ingresos',
    claseColor: 'color-amarillo'
  },
  {
    titulo: 'Facturas',
    icono: 'document-text-outline',
    ruta: '/facturas',
    claseColor: 'color-morado',
    soloAdministrador: true
  }
];

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router
  ) {
    addIcons({
      speedometerOutline,
      peopleOutline,
      carSportOutline,
      businessOutline,
      gridOutline,
      documentTextOutline,
      personCircleOutline,
      barChartOutline,
      settingsOutline,
      logOutOutline,
      pricetagOutline
    });

    this.actualizarVisibilidadMenu(this.router.url);

    this.router.events
      .pipe(
        filter(
          evento => evento instanceof NavigationEnd
        )
      )
      .subscribe(evento => {
        const navegacion = evento as NavigationEnd;

        this.actualizarVisibilidadMenu(
          navegacion.urlAfterRedirects
        );

        if (this.mostrarMenu) {
          void this.actualizarUsuario();
        }
      });
  }

  async ngOnInit(): Promise<void> {
    await this.actualizarUsuario();
  }

  get opcionesVisibles(): OpcionMenu[] {
    return this.opcionesMenu.filter(opcion =>
      !opcion.soloAdministrador ||
      this.esAdministrador
    );
  }

  get inicialesUsuario(): string {
    if (!this.usuario?.nombreCompleto) {
      return 'US';
    }

    const partes = this.usuario.nombreCompleto
      .trim()
      .split(/\s+/)
      .filter(Boolean);

    return partes
      .slice(0, 2)
      .map(parte =>
        parte.charAt(0).toUpperCase()
      )
      .join('');
  }

  async actualizarUsuario(): Promise<void> {
    this.usuario =
      await this.authService.obtenerUsuario();

    this.esAdministrador =
      await this.authService.esAdministrador();
  }

  cerrarSesion(): void {
    void this.authService.cerrarSesion();
  }

  private actualizarVisibilidadMenu(
    ruta: string
  ): void {
    this.mostrarMenu =
      !ruta.startsWith('/login');
  }
}