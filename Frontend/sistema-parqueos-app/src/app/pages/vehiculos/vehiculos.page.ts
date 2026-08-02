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
import { Router } from '@angular/router';

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
  IonToggle, IonHeader, IonToolbar, IonTitle } from '@ionic/angular/standalone';

import { addIcons } from 'ionicons';
import {
  addOutline,
  arrowBackOutline,
  carSportOutline,
  checkmarkCircleOutline,
  closeCircleOutline,
  colorPaletteOutline,
  createOutline,
  peopleOutline,
  pricetagOutline,
  refreshOutline,
  saveOutline,
  trashOutline
} from 'ionicons/icons';

import Swal from 'sweetalert2';

import { Cliente } from '../../models/cliente.model';
import { TipoVehiculo } from '../../models/tipo-vehiculo.model';
import {
  Vehiculo,
  VehiculoActualizar,
  VehiculoCrear
} from '../../models/vehiculo.model';

import { ClienteService } from '../../services/cliente.service';
import { TipoVehiculoService } from '../../services/tipo-vehiculo.service';
import { VehiculoService } from '../../services/vehiculo.service';

@Component({
  selector: 'app-vehiculos',
  templateUrl: './vehiculos.page.html',
  styleUrls: ['./vehiculos.page.scss'],
  standalone: true,
  imports: [IonTitle, IonToolbar, IonHeader, 
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
export class VehiculosPage implements OnInit {

  vehiculos: Vehiculo[] = [];
  vehiculosFiltrados: Vehiculo[] = [];

  clientes: Cliente[] = [];
  tiposVehiculo: TipoVehiculo[] = [];

  formulario: FormGroup;

  textoBusqueda = '';
  cargando = false;
  guardando = false;
  mostrarFormulario = false;
  modoEdicion = false;

  vehiculoSeleccionadoId: number | null = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly vehiculoService: VehiculoService,
    private readonly clienteService: ClienteService,
    private readonly tipoVehiculoService:
      TipoVehiculoService,
    private readonly router: Router
  ) {
    addIcons({
      addOutline,
      arrowBackOutline,
      carSportOutline,
      checkmarkCircleOutline,
      closeCircleOutline,
      colorPaletteOutline,
      createOutline,
      peopleOutline,
      pricetagOutline,
      refreshOutline,
      saveOutline,
      trashOutline
    });

    this.formulario =
      this.formBuilder.group({
        clienteId: [
          null,
          [
            Validators.required,
            Validators.min(1)
          ]
        ],

        tipoVehiculoId: [
          null,
          [
            Validators.required,
            Validators.min(1)
          ]
        ],

        placa: [
          '',
          [
            Validators.required,
            Validators.maxLength(20)
          ]
        ],

        marca: [
          '',
          [
            Validators.required,
            Validators.maxLength(100)
          ]
        ],

        modelo: [
          '',
          [
            Validators.maxLength(100)
          ]
        ],

        color: [
          '',
          [
            Validators.maxLength(50)
          ]
        ],

        activo: [true]
      });
  }

  ngOnInit(): void {
    this.cargarDatosIniciales();
  }

  cargarDatosIniciales(): void {
    this.cargarClientes();
    this.cargarTiposVehiculo();
    this.cargarVehiculos();
  }

  cargarVehiculos(): void {
    this.cargando = true;

    this.vehiculoService
      .obtenerTodos()
      .subscribe({
        next: respuesta => {
          this.vehiculos =
            respuesta.valorRetorno ?? [];

          this.filtrarVehiculos();
          this.cargando = false;
        },

        error: error => {
          this.cargando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible consultar los vehículos.';

          void this.mostrarError(mensaje);
        }
      });
  }

  cargarClientes(): void {
    this.clienteService
      .obtenerTodos()
      .subscribe({
        next: respuesta => {
          this.clientes =
            (respuesta.valorRetorno ?? [])
              .filter(cliente =>
                cliente.activo
              );
        },

        error: error => {
          console.error(
            'Error al cargar clientes:',
            error
          );
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
              .filter(tipo =>
                tipo.activo
              );
        },

        error: error => {
          console.error(
            'Error al cargar tipos:',
            error
          );
        }
      });
  }

  filtrarVehiculos(): void {
    const texto =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    if (!texto) {
      this.vehiculosFiltrados =
        [...this.vehiculos];

      return;
    }

    this.vehiculosFiltrados =
      this.vehiculos.filter(vehiculo =>
        vehiculo.placa
          .toLowerCase()
          .includes(texto) ||

        vehiculo.marca
          .toLowerCase()
          .includes(texto) ||

        (vehiculo.modelo ?? '')
          .toLowerCase()
          .includes(texto) ||

        (vehiculo.color ?? '')
          .toLowerCase()
          .includes(texto) ||

        vehiculo.nombreCliente
          .toLowerCase()
          .includes(texto) ||

        vehiculo.tipoVehiculo
          .toLowerCase()
          .includes(texto)
      );
  }

  abrirFormularioNuevo(): void {
    this.modoEdicion = false;
    this.vehiculoSeleccionadoId = null;

    this.formulario.reset({
      clienteId: null,
      tipoVehiculoId: null,
      placa: '',
      marca: '',
      modelo: '',
      color: '',
      activo: true
    });

    this.mostrarFormulario = true;
  }

  editarVehiculo(
    vehiculo: Vehiculo
  ): void {

    this.modoEdicion = true;

    this.vehiculoSeleccionadoId =
      vehiculo.vehiculoId;

    this.formulario.patchValue({
      clienteId: vehiculo.clienteId,
      tipoVehiculoId:
        vehiculo.tipoVehiculoId,
      placa: vehiculo.placa,
      marca: vehiculo.marca,
      modelo: vehiculo.modelo ?? '',
      color: vehiculo.color ?? '',
      activo: vehiculo.activo
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
    this.vehiculoSeleccionadoId = null;
  }

  guardarVehiculo(): void {
    this.formulario.markAllAsTouched();

    if (this.formulario.invalid) {
      void Swal.fire({
        icon: 'warning',
        title: 'Formulario incompleto',
        text:
          'Revise los campos obligatorios antes de continuar.',
        confirmButtonColor: '#7cb928',
        background: '#1c2325',
        color: '#f4f7f3'
      });

      return;
    }

    this.guardando = true;

    if (
      this.modoEdicion &&
      this.vehiculoSeleccionadoId
    ) {
      this.actualizarVehiculo();
      return;
    }

    this.crearVehiculo();
  }

  async desactivarVehiculo(
    vehiculo: Vehiculo
  ): Promise<void> {

    if (!vehiculo.activo) {
      await Swal.fire({
        icon: 'info',
        title: 'Vehículo inactivo',
        text:
          'Este vehículo ya se encuentra desactivado.',
        confirmButtonColor: '#7cb928',
        background: '#1c2325',
        color: '#f4f7f3'
      });

      return;
    }

    const confirmacion =
      await Swal.fire({
        icon: 'warning',
        title: '¿Desactivar vehículo?',
        html:
          `Se desactivará el vehículo con placa <strong>${vehiculo.placa}</strong>.`,
        showCancelButton: true,
        confirmButtonText: 'Sí, desactivar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#7cb928',
        cancelButtonColor: '#555b57',
        background: '#1c2325',
        color: '#f4f7f3'
      });

    if (!confirmacion.isConfirmed) {
      return;
    }

    this.vehiculoService
      .eliminar(vehiculo.vehiculoId)
      .subscribe({
        next: respuesta => {
          void this.mostrarExito(
            'Vehículo desactivado',
            respuesta.mensaje
          );

          this.cargarVehiculos();
        },

        error: error => {
          const mensaje =
            error?.error?.mensaje ??
            'No fue posible desactivar el vehículo.';

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

  regresar(): void {
    void this.router.navigateByUrl(
      '/dashboard'
    );
  }

  get totalActivos(): number {
    return this.vehiculos.filter(
      vehiculo => vehiculo.activo
    ).length;
  }

  get totalInactivos(): number {
    return this.vehiculos.filter(
      vehiculo => !vehiculo.activo
    ).length;
  }

  private crearVehiculo(): void {
    const valor = this.formulario.value;

    const vehiculo: VehiculoCrear = {
      clienteId:
        Number(valor.clienteId),

      tipoVehiculoId:
        Number(valor.tipoVehiculoId),

      placa:
        String(valor.placa)
          .trim()
          .toUpperCase(),

      marca:
        String(valor.marca).trim(),

      modelo:
        this.limpiarOpcional(
          valor.modelo
        ),

      color:
        this.limpiarOpcional(
          valor.color
        )
    };

    this.vehiculoService
      .agregar(vehiculo)
      .subscribe({
        next: respuesta => {
          this.guardando = false;
          this.mostrarFormulario = false;

          void this.mostrarExito(
            'Vehículo registrado',
            respuesta.mensaje
          );

          this.cargarVehiculos();
        },

        error: error => {
          this.guardando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible registrar el vehículo.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private actualizarVehiculo(): void {
    if (!this.vehiculoSeleccionadoId) {
      this.guardando = false;
      return;
    }

    const valor = this.formulario.value;

    const vehiculo:
      VehiculoActualizar = {

      clienteId:
        Number(valor.clienteId),

      tipoVehiculoId:
        Number(valor.tipoVehiculoId),

      placa:
        String(valor.placa)
          .trim()
          .toUpperCase(),

      marca:
        String(valor.marca).trim(),

      modelo:
        this.limpiarOpcional(
          valor.modelo
        ),

      color:
        this.limpiarOpcional(
          valor.color
        ),

      activo:
        Boolean(valor.activo)
    };

    this.vehiculoService
      .actualizar(
        this.vehiculoSeleccionadoId,
        vehiculo
      )
      .subscribe({
        next: respuesta => {
          this.guardando = false;
          this.mostrarFormulario = false;
          this.modoEdicion = false;
          this.vehiculoSeleccionadoId = null;

          void this.mostrarExito(
            'Vehículo actualizado',
            respuesta.mensaje
          );

          this.cargarVehiculos();
        },

        error: error => {
          this.guardando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible actualizar el vehículo.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private limpiarOpcional(
    valor: unknown
  ): string | null {

    const texto =
      String(valor ?? '').trim();

    return texto || null;
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

  private async mostrarError(
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'error',
      title: 'Ocurrió un problema',
      text: mensaje,
      confirmButtonColor: '#7cb928',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }
}