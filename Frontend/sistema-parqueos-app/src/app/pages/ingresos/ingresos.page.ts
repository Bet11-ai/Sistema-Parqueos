import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import {
  IonBadge,
  IonButton,
  IonContent,
  IonIcon,
  IonItem,
  IonSearchbar,
  IonSelect,
  IonSelectOption,
  IonSpinner
} from '@ionic/angular/standalone';
import { addIcons } from 'ionicons';
import {
  arrowDownCircleOutline,
  carSportOutline,
  checkmarkCircleOutline,
  closeCircleOutline,
  documentTextOutline,
  enterOutline,
  exitOutline,
  locationOutline,
  refreshOutline
} from 'ionicons/icons';
import Swal from 'sweetalert2';

import { Vehiculo } from '../../models/vehiculo.model';
import { Parqueo } from '../../models/parqueo.model';
import { EspacioParqueo } from '../../models/espacio-parqueo.model';
import {
  IngresoVehiculo,
  IngresoVehiculoCrear,
  SalidaVehiculoRespuesta
} from '../../models/ingreso-vehiculo.model';
import { VehiculoService } from '../../services/vehiculo.service';
import { ParqueoService } from '../../services/parqueo.service';
import { EspacioParqueoService } from '../../services/espacio-parqueo.service';
import { IngresoVehiculoService } from '../../services/ingreso-vehiculo.service';

@Component({
  selector: 'app-ingresos',
  templateUrl: './ingresos.page.html',
  styleUrls: ['./ingresos.page.scss'],
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    IonContent,
    IonButton,
    IonIcon,
    IonItem,
    IonSelect,
    IonSelectOption,
    IonSearchbar,
    IonSpinner,
    IonBadge
  ]
})
export class IngresosPage implements OnInit {
  formularioIngreso: FormGroup;
  vehiculos: Vehiculo[] = [];
  parqueos: Parqueo[] = [];
  espaciosDisponibles: EspacioParqueo[] = [];
  ingresos: IngresoVehiculo[] = [];
  ingresosActivos: IngresoVehiculo[] = [];
  historialFiltrado: IngresoVehiculo[] = [];
  textoBusqueda = '';
  cargando = false;
  guardando = false;
  registrandoSalidaId: number | null = null;
  mostrarFormulario = false;

  constructor(
    private readonly fb: FormBuilder,
    private readonly vehiculoService: VehiculoService,
    private readonly parqueoService: ParqueoService,
    private readonly espacioService: EspacioParqueoService,
    private readonly ingresoService: IngresoVehiculoService
  ) {
    addIcons({
      arrowDownCircleOutline,
      carSportOutline,
      checkmarkCircleOutline,
      closeCircleOutline,
      documentTextOutline,
      enterOutline,
      exitOutline,
      locationOutline,
      refreshOutline
    });

    this.formularioIngreso = this.fb.group({
      vehiculoId: [null, [Validators.required, Validators.min(1)]],
      parqueoId: [null, [Validators.required, Validators.min(1)]],
      espacioId: [null, [Validators.required, Validators.min(1)]]
    });
  }

  ngOnInit(): void {
    this.cargarDatosIniciales();
  }

  cargarDatosIniciales(): void {
    this.cargarParqueos();
    this.cargarIngresos();
    this.cargarActivos();
  }

  cargarVehiculos(): void {
    this.vehiculoService.obtenerTodos().subscribe({
      next: respuesta => {
        const todos = respuesta.valorRetorno ?? [];
        this.vehiculos = todos.filter(
          vehiculo => vehiculo.activo &&
            !this.ingresosActivos.some(activo => activo.vehiculoId === vehiculo.vehiculoId)
        );
      },
      error: error => console.error('Error al cargar vehículos:', error)
    });
  }

  cargarParqueos(): void {
    this.parqueoService.obtenerTodos().subscribe({
      next: respuesta => {
        this.parqueos = (respuesta.valorRetorno ?? []).filter(parqueo => parqueo.activo);
      },
      error: error => console.error('Error al cargar parqueos:', error)
    });
  }

  cargarEspacios(parqueoId: number): void {
    this.espaciosDisponibles = [];
    this.formularioIngreso.patchValue({ espacioId: null });
    if (!parqueoId) return;

    this.espacioService.obtenerPorParqueo(parqueoId).subscribe({
      next: respuesta => {
        this.espaciosDisponibles = (respuesta.valorRetorno ?? [])
          .filter(espacio => espacio.activo && espacio.disponible);
      },
      error: error => void this.mostrarError(
        error?.error?.mensaje ?? 'No fue posible consultar los espacios disponibles.'
      )
    });
  }

  seleccionarParqueo(): void {
    this.cargarEspacios(Number(this.formularioIngreso.get('parqueoId')?.value));
  }

  cargarIngresos(): void {
    this.cargando = true;
    this.ingresoService.obtenerTodos().subscribe({
      next: respuesta => {
        this.ingresos = respuesta.valorRetorno ?? [];
        this.filtrarHistorial();
        this.cargando = false;
      },
      error: error => {
        this.cargando = false;
        void this.mostrarError(error?.error?.mensaje ?? 'No fue posible consultar los ingresos.');
      }
    });
  }

  cargarActivos(): void {
    this.ingresoService.obtenerActivos().subscribe({
      next: respuesta => {
        this.ingresosActivos = respuesta.valorRetorno ?? [];
        this.cargarVehiculos();
      },
      error: error => void this.mostrarError(
        error?.error?.mensaje ?? 'No fue posible consultar los vehículos dentro.'
      )
    });
  }

  abrirFormulario(): void {
    this.formularioIngreso.reset({ vehiculoId: null, parqueoId: null, espacioId: null });
    this.espaciosDisponibles = [];
    this.mostrarFormulario = true;
  }

  cancelarFormulario(): void {
    if (!this.guardando) this.mostrarFormulario = false;
  }

  registrarIngreso(): void {
    this.formularioIngreso.markAllAsTouched();
    if (this.formularioIngreso.invalid) {
      void this.mostrarAdvertencia('Seleccione el vehículo, el parqueo y un espacio disponible.');
      return;
    }

    const valor = this.formularioIngreso.value;
    const ingreso: IngresoVehiculoCrear = {
      vehiculoId: Number(valor.vehiculoId),
      espacioId: Number(valor.espacioId)
    };

    this.guardando = true;
    this.ingresoService.agregar(ingreso).subscribe({
      next: respuesta => {
        this.guardando = false;
        this.mostrarFormulario = false;
        void this.mostrarExito('Ingreso registrado', respuesta.mensaje);
        this.cargarActivos();
        this.cargarIngresos();
      },
      error: error => {
        this.guardando = false;
        void this.mostrarError(error?.error?.mensaje ?? 'No fue posible registrar el ingreso.');
      }
    });
  }

  async registrarSalida(ingreso: IngresoVehiculo): Promise<void> {
    const confirmacion = await Swal.fire({
      icon: 'question',
      title: 'Registrar salida',
      html: `¿Desea registrar la salida del vehículo <strong>${ingreso.placa}</strong>?`,
      showCancelButton: true,
      confirmButtonText: 'Sí, registrar salida',
      cancelButtonText: 'Cancelar',
      confirmButtonColor: '#11d8c4',
      cancelButtonColor: '#555b57',
      background: '#1c2325',
      color: '#f4f7f3'
    });

    if (!confirmacion.isConfirmed) return;

    this.registrandoSalidaId = ingreso.ingresoId;
    this.ingresoService.registrarSalida(ingreso.ingresoId).subscribe({
      next: respuesta => {
        this.registrandoSalidaId = null;
        if (respuesta.valorRetorno) {
          void this.mostrarComprobanteSalida(respuesta.valorRetorno);
        } else {
          void this.mostrarError(respuesta.mensaje);
        }
        this.cargarActivos();
        this.cargarIngresos();
      },
      error: error => {
        this.registrandoSalidaId = null;
        void this.mostrarError(error?.error?.mensaje ?? 'No fue posible registrar la salida.');
      }
    });
  }

  filtrarHistorial(): void {
    const texto = this.textoBusqueda.trim().toLowerCase();
    this.historialFiltrado = !texto
      ? [...this.ingresos]
      : this.ingresos.filter(ingreso =>
          ingreso.placa.toLowerCase().includes(texto) ||
          ingreso.numeroEspacio.toLowerCase().includes(texto) ||
          ingreso.nombreParqueo.toLowerCase().includes(texto) ||
          ingreso.estado.toLowerCase().includes(texto)
        );
  }

  campoInvalido(campo: string): boolean {
    const control = this.formularioIngreso.get(campo);
    return Boolean(control && control.invalid && control.touched);
  }

  formatearFecha(fecha: string | null): string {
    if (!fecha) return 'Sin registrar';
    return new Intl.DateTimeFormat('es-CR', {
      day: '2-digit',
      month: '2-digit',
      year: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(new Date(fecha));
  }

  formatearMoneda(monto: number): string {
    return new Intl.NumberFormat('es-CR', {
      style: 'currency',
      currency: 'CRC',
      maximumFractionDigits: 0
    }).format(monto);
  }

  get totalIngresosHoy(): number {
    const hoy = new Date().toDateString();
    return this.ingresos.filter(
      ingreso => new Date(ingreso.fechaIngreso).toDateString() === hoy
    ).length;
  }

  get totalFinalizados(): number {
    return this.ingresos.filter(ingreso => ingreso.fechaSalida !== null).length;
  }

  private async mostrarComprobanteSalida(salida: SalidaVehiculoRespuesta): Promise<void> {
    await Swal.fire({
      icon: 'success',
      title: 'Salida registrada',
      html: `<div style="text-align:left;line-height:1.8">
        <p><strong>Placa:</strong> ${salida.placa}</p>
        <p><strong>Espacio:</strong> ${salida.numeroEspacio}</p>
        <p><strong>Horas cobradas:</strong> ${salida.horasCobradas}</p>
        <p><strong>Monto por hora:</strong> ${this.formatearMoneda(salida.montoHora)}</p>
        <p><strong>Total:</strong> ${this.formatearMoneda(salida.montoTotal)}</p>
        <p><strong>Factura:</strong> #${salida.facturaId}</p>
      </div>`,
      confirmButtonColor: '#11d8c4',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }

  private async mostrarExito(titulo: string, mensaje: string): Promise<void> {
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

  private async mostrarAdvertencia(mensaje: string): Promise<void> {
    await Swal.fire({
      icon: 'warning',
      title: 'Revise la información',
      text: mensaje,
      confirmButtonColor: '#f0b51b',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }

  private async mostrarError(mensaje: string): Promise<void> {
    await Swal.fire({
      icon: 'error',
      title: 'Ocurrió un problema',
      text: mensaje,
      confirmButtonColor: '#f0b51b',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }
}