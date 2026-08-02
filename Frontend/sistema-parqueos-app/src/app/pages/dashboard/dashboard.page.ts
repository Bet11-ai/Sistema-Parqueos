import { CommonModule } from '@angular/common';
import {
  AfterViewInit,
  Component,
  ElementRef,
  OnDestroy,
  OnInit,
  ViewChild
} from '@angular/core';

import {
  IonButton,
  IonContent,
  IonIcon,
  IonSpinner, IonHeader, IonToolbar } from '@ionic/angular/standalone';

import { addIcons } from 'ionicons';
import {
  arrowForwardOutline,
  businessOutline,
  carSportOutline,
  cashOutline,
  checkmarkCircleOutline,
  documentTextOutline,
  gridOutline,
  peopleOutline,
  refreshOutline,
  timeOutline,
  trendingUpOutline
} from 'ionicons/icons';

import {
  Chart,
  ChartConfiguration,
  DoughnutController,
  ArcElement,
  Tooltip,
  Legend
} from 'chart.js';

import Swal from 'sweetalert2';

import {
  ActividadReciente,
  DashboardResumen
} from '../../models/dashboard.model';

import { DashboardService } from '../../services/dashboard.service';
import { AuthService } from '../../services/auth.service';
import { UsuarioAutenticado } from '../../models/auth.model';

Chart.register(
  DoughnutController,
  ArcElement,
  Tooltip,
  Legend
);

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.page.html',
  styleUrls: ['./dashboard.page.scss'],
  standalone: true,
  imports: [IonToolbar, IonHeader, 
    CommonModule,
    IonContent,
    IonIcon,
    IonButton,
    IonSpinner
  ]
})
export class DashboardPage
implements OnInit, AfterViewInit, OnDestroy {

  @ViewChild('graficoOcupacion')
  graficoOcupacion?: ElementRef<HTMLCanvasElement>;

  usuario: UsuarioAutenticado | null = null;

  resumen: DashboardResumen = {
    vehiculosDentro: 0,
    espaciosDisponibles: 0,
    espaciosOcupados: 0,
    totalEspaciosActivos: 0,
    ingresosHoy: 0,
    facturacionHoy: 0,
    facturacionMes: 0,
    clientesActivos: 0,
    vehiculosActivos: 0,
    porcentajeOcupacion: 0,
    actividadReciente: []
  };

  cargando = false;
  vistaInicializada = false;

  private grafico?: Chart;

  constructor(
    private readonly dashboardService:
      DashboardService,
    private readonly authService:
      AuthService
  ) {
    addIcons({
      carSportOutline,
      checkmarkCircleOutline,
      gridOutline,
      cashOutline,
      trendingUpOutline,
      peopleOutline,
      businessOutline,
      documentTextOutline,
      refreshOutline,
      timeOutline,
      arrowForwardOutline
    });
  }

  async ngOnInit(): Promise<void> {
    this.usuario =
      await this.authService.obtenerUsuario();

    this.cargarResumen();
  }

  ngAfterViewInit(): void {
    this.vistaInicializada = true;
    this.crearGrafico();
  }

  ngOnDestroy(): void {
    this.grafico?.destroy();
  }

  cargarResumen(): void {
    this.cargando = true;

    this.dashboardService
      .obtenerResumen()
      .subscribe({
        next: respuesta => {
          this.cargando = false;

          if (
            !respuesta.exito ||
            !respuesta.valorRetorno
          ) {
            void this.mostrarError(
              respuesta.mensaje
            );

            return;
          }

          this.resumen =
            respuesta.valorRetorno;

          this.crearGrafico();
        },

        error: error => {
          this.cargando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible consultar el resumen.';

          void this.mostrarError(mensaje);
        }
      });
  }

  get actividadReciente():
    ActividadReciente[] {

    return this.resumen.actividadReciente ?? [];
  }

  get porcentajeDisponible(): number {
    if (
      this.resumen.totalEspaciosActivos <= 0
    ) {
      return 0;
    }

    return Math.round(
      (
        this.resumen.espaciosDisponibles /
        this.resumen.totalEspaciosActivos
      ) * 100
    );
  }

  formatearMoneda(
    monto: number
  ): string {

    return new Intl.NumberFormat(
      'es-CR',
      {
        style: 'currency',
        currency: 'CRC',
        maximumFractionDigits: 0
      }
    ).format(monto);
  }

 formatearFecha(
  fecha: string | null
): string {

  if (!fecha) {
    return 'Sin registrar';
  }

  return new Intl.DateTimeFormat(
    'es-CR',
    {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }
  ).format(new Date(fecha));
}

  calcularIniciales(
    texto: string
  ): string {

    if (!texto?.trim()) {
      return 'NA';
    }

    return texto
      .trim()
      .split(/\s+/)
      .slice(0, 2)
      .map(parte =>
        parte.charAt(0).toUpperCase()
      )
      .join('');
  }

  private crearGrafico(): void {
    if (
      !this.vistaInicializada ||
      !this.graficoOcupacion
    ) {
      return;
    }

    this.grafico?.destroy();

    const configuracion:
      ChartConfiguration<'doughnut'> = {

      type: 'doughnut',

      data: {
        labels: [
          'Ocupados',
          'Disponibles'
        ],

        datasets: [
          {
            data: [
              this.resumen.espaciosOcupados,
              this.resumen.espaciosDisponibles
            ],

            backgroundColor: [
              '#f0b51b',
              '#11d8c4'
            ],

            borderColor: [
              '#1c2325',
              '#1c2325'
            ],

            borderWidth: 4,
            hoverOffset: 7
          }
        ]
      },

      options: {
        responsive: true,
        maintainAspectRatio: false,
        cutout: '73%',

        plugins: {
          legend: {
            position: 'bottom',

            labels: {
              color: '#c9d0cd',
              padding: 20,
              usePointStyle: true,
              pointStyle: 'circle'
            }
          },

          tooltip: {
            callbacks: {
              label: contexto =>
                `${contexto.label}: ${contexto.raw}`
            }
          }
        }
      }
    };

    this.grafico =
      new Chart(
        this.graficoOcupacion
          .nativeElement,
        configuracion
      );
  }

  private async mostrarError(
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'error',
      title: 'No se pudo cargar',
      text: mensaje,
      confirmButtonColor: '#7cb928',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }
}