import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit
} from '@angular/core';

import {
  IonBadge,
  IonButton,
  IonContent,
  IonIcon,
  IonSearchbar,
  IonSpinner, IonHeader, IonToolbar, IonTitle } from '@ionic/angular/standalone';

import { addIcons } from 'ionicons';

import {
  businessOutline,
  calendarOutline,
  carSportOutline,
  cashOutline,
  closeCircleOutline,
  documentTextOutline,
  eyeOutline,
  locationOutline,
  refreshOutline,
  timeOutline,
  trendingUpOutline
} from 'ionicons/icons';

import Swal from 'sweetalert2';

import {
  Factura
} from '../../models/factura.model';

import {
  FacturaService
} from '../../services/factura.service';

@Component({
  selector: 'app-facturas',
  templateUrl: './facturas.page.html',
  styleUrls: ['./facturas.page.scss'],
  standalone: true,
  imports: [IonTitle, IonToolbar, IonHeader, 
    CommonModule,
    IonContent,
    IonButton,
    IonIcon,
    IonSearchbar,
    IonSpinner,
    IonBadge
  ]
})
export class FacturasPage implements OnInit {

  facturas: Factura[] = [];
  facturasFiltradas: Factura[] = [];

  textoBusqueda = '';
  cargando = false;

  constructor(
    private readonly facturaService:
      FacturaService
  ) {
    addIcons({
      businessOutline,
      calendarOutline,
      carSportOutline,
      cashOutline,
      closeCircleOutline,
      documentTextOutline,
      eyeOutline,
      locationOutline,
      refreshOutline,
      timeOutline,
      trendingUpOutline
    });
  }

  ngOnInit(): void {
    this.cargarFacturas();
  }

  cargarFacturas(): void {
    this.cargando = true;

    this.facturaService
      .obtenerTodas()
      .subscribe({
        next: respuesta => {
          this.facturas =
            respuesta.valorRetorno ?? [];

          this.filtrarFacturas();

          this.cargando = false;
        },

        error: error => {
          this.cargando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible consultar las facturas.';

          void this.mostrarError(mensaje);
        }
      });
  }

  filtrarFacturas(): void {
    const texto =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    if (!texto) {
      this.facturasFiltradas =
        [...this.facturas];

      return;
    }

    this.facturasFiltradas =
      this.facturas.filter(factura =>
        factura.facturaId
          .toString()
          .includes(texto) ||

        factura.ingresoId
          .toString()
          .includes(texto) ||

        factura.placa
          .toLowerCase()
          .includes(texto) ||

        factura.tipoVehiculo
          .toLowerCase()
          .includes(texto) ||

        factura.numeroEspacio
          .toLowerCase()
          .includes(texto) ||

        factura.nombreParqueo
          .toLowerCase()
          .includes(texto)
      );
  }

  async verDetalle(
    factura: Factura
  ): Promise<void> {

    await Swal.fire({
      title:
        `Factura #${factura.facturaId}`,

      html: `
        <div class="invoice-detail">

          <div class="invoice-total">
            <span>Monto total</span>

            <strong>
              ${this.formatearMoneda(
                factura.montoTotal
              )}
            </strong>
          </div>

          <div class="invoice-detail-grid">

            <div>
              <span>Ingreso</span>
              <strong>
                #${factura.ingresoId}
              </strong>
            </div>

            <div>
              <span>Placa</span>
              <strong>
                ${this.escaparHtml(
                  factura.placa
                )}
              </strong>
            </div>

            <div>
              <span>Tipo de vehículo</span>
              <strong>
                ${this.escaparHtml(
                  factura.tipoVehiculo
                )}
              </strong>
            </div>

            <div>
              <span>Parqueo</span>
              <strong>
                ${this.escaparHtml(
                  factura.nombreParqueo
                )}
              </strong>
            </div>

            <div>
              <span>Espacio</span>
              <strong>
                ${this.escaparHtml(
                  factura.numeroEspacio
                )}
              </strong>
            </div>

            <div>
              <span>Horas cobradas</span>
              <strong>
                ${factura.horasCobradas}
              </strong>
            </div>

            <div>
              <span>Fecha de ingreso</span>
              <strong>
                ${this.formatearFecha(
                  factura.fechaIngreso
                )}
              </strong>
            </div>

            <div>
              <span>Fecha de salida</span>
              <strong>
                ${this.formatearFecha(
                  factura.fechaSalida
                )}
              </strong>
            </div>

            <div class="invoice-wide">
              <span>Fecha de factura</span>
              <strong>
                ${this.formatearFecha(
                  factura.fechaFactura
                )}
              </strong>
            </div>

          </div>

        </div>
      `,

      confirmButtonText: 'Cerrar',
      confirmButtonColor: '#a66cf4',
      background: '#1c2325',
      color: '#f4f7f3',
      width: 650,

      customClass: {
        popup: 'invoice-popup'
      }
    });
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

  get totalFacturado(): number {
    return this.facturas.reduce(
      (total, factura) =>
        total + factura.montoTotal,
      0
    );
  }

  get facturacionHoy(): number {
    const hoy =
      new Date().toDateString();

    return this.facturas
      .filter(
        factura =>
          new Date(
            factura.fechaFactura
          ).toDateString() === hoy
      )
      .reduce(
        (total, factura) =>
          total + factura.montoTotal,
        0
      );
  }

  get facturasHoy(): number {
    const hoy =
      new Date().toDateString();

    return this.facturas.filter(
      factura =>
        new Date(
          factura.fechaFactura
        ).toDateString() === hoy
    ).length;
  }

  get promedioFactura(): number {
    if (this.facturas.length === 0) {
      return 0;
    }

    return (
      this.totalFacturado /
      this.facturas.length
    );
  }

  private escaparHtml(
    valor: string
  ): string {

    return valor
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#039;');
  }

  private async mostrarError(
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'error',
      title: 'Ocurrió un problema',
      text: mensaje,
      confirmButtonColor: '#a66cf4',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }
}