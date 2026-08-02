import { CommonModule } from '@angular/common';
import {
  Component,
  OnInit
} from '@angular/core';

import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import {
  IonBadge,
  IonButton,
  IonContent,
  IonIcon,
  IonInput,
  IonItem,
  IonLabel,
  IonSearchbar,
  IonSelect,
  IonSelectOption,
  IonSpinner,
  IonToggle
} from '@ionic/angular/standalone';

import { addIcons } from 'ionicons';
import {
  addOutline,
  cashOutline,
  checkmarkCircleOutline,
  closeCircleOutline,
  createOutline,
  pricetagOutline,
  refreshOutline,
  saveOutline,
  timeOutline,
  trashOutline
} from 'ionicons/icons';

import Swal from 'sweetalert2';

import {
  ActualizarTarifa,
  CrearTarifa,
  Tarifa
} from '../../models/tarifa.model';

import {
  TipoVehiculo
} from '../../models/tipo-vehiculo.model';

import {
  TarifaService
} from '../../services/tarifa.service';

import {
  TipoVehiculoService
} from '../../services/tipo-vehiculo.service';

@Component({
  selector: 'app-tarifas',
  templateUrl: './tarifas.page.html',
  styleUrls: ['./tarifas.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    IonContent,
    IonButton,
    IonIcon,
    IonInput,
    IonItem,
    IonLabel,
    IonSelect,
    IonSelectOption,
    IonSearchbar,
    IonSpinner,
    IonBadge,
    IonToggle
  ]
})
export class TarifasPage implements OnInit {

  tarifas: Tarifa[] = [];
  tarifasFiltradas: Tarifa[] = [];
  tiposVehiculo: TipoVehiculo[] = [];

  formulario: FormGroup;

  textoBusqueda = '';
  cargando = false;
  guardando = false;
  mostrarFormulario = false;
  modoEdicion = false;

  tarifaSeleccionadaId: number | null = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly tarifaService: TarifaService,
    private readonly tipoVehiculoService:
      TipoVehiculoService
  ) {
    addIcons({
      addOutline,
      cashOutline,
      checkmarkCircleOutline,
      closeCircleOutline,
      createOutline,
      pricetagOutline,
      refreshOutline,
      saveOutline,
      timeOutline,
      trashOutline
    });

    this.formulario =
      this.formBuilder.group({
        tipoVehiculoId: [
          null,
          [
            Validators.required,
            Validators.min(1)
          ]
        ],

        descripcion: [
          '',
          [
            Validators.required,
            Validators.maxLength(100)
          ]
        ],

        montoHora: [
          0,
          [
            Validators.required,
            Validators.min(0.01)
          ]
        ],

        activo: [true]
      });
  }

  ngOnInit(): void {
    this.cargarTiposVehiculo();
    this.cargarTarifas();
  }

  cargarTarifas(): void {
    this.cargando = true;

    this.tarifaService
      .obtenerTodos()
      .subscribe({
        next: respuesta => {
          this.tarifas =
            respuesta.valorRetorno ?? [];

          this.filtrarTarifas();

          this.cargando = false;
        },

        error: error => {
          this.cargando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible consultar las tarifas.';

          void this.mostrarError(mensaje);
        }
      });
  }

  cargarTiposVehiculo(): void {
    this.tipoVehiculoService
      .obtenerTodos()
      .subscribe({
        next: respuesta => {
          this.tiposVehiculo =
            (respuesta.valorRetorno ?? [])
              .filter(tipo => tipo.activo);
        },

        error: error => {
          console.error(
            'No fue posible cargar los tipos de vehículo.',
            error
          );
        }
      });
  }

  filtrarTarifas(): void {
    const texto =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    if (!texto) {
      this.tarifasFiltradas =
        [...this.tarifas];

      return;
    }

    this.tarifasFiltradas =
      this.tarifas.filter(tarifa =>
        tarifa.descripcion
          .toLowerCase()
          .includes(texto) ||

        tarifa.tipoVehiculo
          .toLowerCase()
          .includes(texto) ||

        tarifa.montoHora
          .toString()
          .includes(texto)
      );
  }

  abrirFormularioNuevo(): void {
    this.modoEdicion = false;
    this.tarifaSeleccionadaId = null;

    this.formulario.reset({
      tipoVehiculoId: null,
      descripcion: '',
      montoHora: 0,
      activo: true
    });

    this.mostrarFormulario = true;
  }

  editarTarifa(
    tarifa: Tarifa
  ): void {

    this.modoEdicion = true;

    this.tarifaSeleccionadaId =
      tarifa.tarifaId;

    this.formulario.patchValue({
      tipoVehiculoId:
        tarifa.tipoVehiculoId,

      descripcion:
        tarifa.descripcion,

      montoHora:
        tarifa.montoHora,

      activo:
        tarifa.activo
    });

    this.mostrarFormulario = true;

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  cancelarFormulario(): void {
    if (this.guardando) {
      return;
    }

    this.mostrarFormulario = false;
    this.modoEdicion = false;
    this.tarifaSeleccionadaId = null;
  }

  guardarTarifa(): void {
    this.formulario.markAllAsTouched();

    if (this.formulario.invalid) {
      void this.mostrarAdvertencia(
        'Revise los campos obligatorios de la tarifa.'
      );

      return;
    }

    this.guardando = true;

    if (
      this.modoEdicion &&
      this.tarifaSeleccionadaId
    ) {
      this.actualizarTarifa();
      return;
    }

    this.crearTarifa();
  }

  async desactivarTarifa(
    tarifa: Tarifa
  ): Promise<void> {

    if (!tarifa.activo) {
      await this.mostrarAdvertencia(
        'Esta tarifa ya se encuentra inactiva.'
      );

      return;
    }

    const confirmacion =
      await Swal.fire({
        icon: 'warning',
        title: '¿Desactivar tarifa?',
        html:
          `Se desactivará <strong>${tarifa.descripcion}</strong>.`,
        showCancelButton: true,
        confirmButtonText: 'Sí, desactivar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#a66cf4',
        cancelButtonColor: '#555b57',
        background: '#1c2325',
        color: '#f4f7f3'
      });

    if (!confirmacion.isConfirmed) {
      return;
    }

    this.tarifaService
      .eliminar(tarifa.tarifaId)
      .subscribe({
        next: respuesta => {
          void this.mostrarExito(
            'Tarifa desactivada',
            respuesta.mensaje
          );

          this.cargarTarifas();
        },

        error: error => {
          const mensaje =
            error?.error?.mensaje ??
            'No fue posible desactivar la tarifa.';

          void this.mostrarError(mensaje);
        }
      });
  }

  campoInvalido(
    campo: string
  ): boolean {

    const control =
      this.formulario.get(campo);

    return Boolean(
      control &&
      control.invalid &&
      control.touched
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

  get totalActivas(): number {
    return this.tarifas.filter(
      tarifa => tarifa.activo
    ).length;
  }

  get totalInactivas(): number {
    return this.tarifas.filter(
      tarifa => !tarifa.activo
    ).length;
  }

  get tarifaPromedio(): number {
    const activas =
      this.tarifas.filter(
        tarifa => tarifa.activo
      );

    if (activas.length === 0) {
      return 0;
    }

    const total =
      activas.reduce(
        (suma, tarifa) =>
          suma + tarifa.montoHora,
        0
      );

    return total / activas.length;
  }

  private crearTarifa(): void {
    const valor =
      this.formulario.value;

    const tarifa: CrearTarifa = {
      tipoVehiculoId:
        Number(valor.tipoVehiculoId),

      descripcion:
        String(valor.descripcion)
          .trim(),

      montoHora:
        Number(valor.montoHora)
    };

    this.tarifaService
      .agregar(tarifa)
      .subscribe({
        next: respuesta => {
          this.guardando = false;
          this.mostrarFormulario = false;

          void this.mostrarExito(
            'Tarifa registrada',
            respuesta.mensaje
          );

          this.cargarTarifas();
        },

        error: error => {
          this.guardando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible registrar la tarifa.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private actualizarTarifa(): void {
    if (!this.tarifaSeleccionadaId) {
      this.guardando = false;
      return;
    }

    const valor =
      this.formulario.value;

    const tarifa: ActualizarTarifa = {
      tipoVehiculoId:
        Number(valor.tipoVehiculoId),

      descripcion:
        String(valor.descripcion)
          .trim(),

      montoHora:
        Number(valor.montoHora),

      activo:
        Boolean(valor.activo)
    };

    this.tarifaService
      .actualizar(
        this.tarifaSeleccionadaId,
        tarifa
      )
      .subscribe({
        next: respuesta => {
          this.guardando = false;
          this.mostrarFormulario = false;
          this.modoEdicion = false;
          this.tarifaSeleccionadaId = null;

          void this.mostrarExito(
            'Tarifa actualizada',
            respuesta.mensaje
          );

          this.cargarTarifas();
        },

        error: error => {
          this.guardando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible actualizar la tarifa.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private async mostrarExito(
    titulo: string,
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'success',
      title: titulo,
      text: mensaje,
      timer: 1400,
      showConfirmButton: false,
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }

  private async mostrarAdvertencia(
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'warning',
      title: 'Revise la información',
      text: mensaje,
      confirmButtonColor: '#a66cf4',
      background: '#1c2325',
      color: '#f4f7f3'
    });
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