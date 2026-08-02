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
  IonSpinner,
  IonToggle, IonHeader, IonToolbar, IonTitle } from '@ionic/angular/standalone';

import { addIcons } from 'ionicons';

import {
  addOutline,
  businessOutline,
  callOutline,
  checkmarkCircleOutline,
  closeCircleOutline,
  createOutline,
  locationOutline,
  refreshOutline,
  saveOutline,
  trashOutline
} from 'ionicons/icons';

import Swal from 'sweetalert2';

import {
  Parqueo,
  ParqueoActualizar,
  ParqueoCrear
} from '../../models/parqueo.model';

import {
  EspacioParqueo,
  EspacioParqueoActualizar,
  EspacioParqueoCrear
} from '../../models/espacio-parqueo.model';

import {
  ParqueoService
} from '../../services/parqueo.service';

import {
  EspacioParqueoService
} from '../../services/espacio-parqueo.service';

@Component({
  selector: 'app-parqueos',
  templateUrl: './parqueos.page.html',
  styleUrls: ['./parqueos.page.scss'],
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
    IonSearchbar,
    IonSpinner,
    IonBadge,
    IonToggle
  ]
})
export class ParqueosPage implements OnInit {

  parqueos: Parqueo[] = [];
  parqueosFiltrados: Parqueo[] = [];

  espacios: EspacioParqueo[] = [];
  espaciosFiltrados: EspacioParqueo[] = [];

  parqueoSeleccionado: Parqueo | null = null;

  formularioParqueo: FormGroup;
  formularioEspacio: FormGroup;

  textoBusquedaParqueo = '';
  textoBusquedaEspacio = '';

  cargandoParqueos = false;
  cargandoEspacios = false;
  guardandoParqueo = false;
  guardandoEspacio = false;

  mostrarFormularioParqueo = false;
  mostrarFormularioEspacio = false;

  modoEdicionParqueo = false;
  modoEdicionEspacio = false;

  parqueoSeleccionadoId: number | null = null;
  espacioSeleccionadoId: number | null = null;

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly parqueoService: ParqueoService,
    private readonly espacioService:
      EspacioParqueoService
  ) {
    addIcons({
      addOutline,
      businessOutline,
      callOutline,
      checkmarkCircleOutline,
      closeCircleOutline,
      createOutline,
      locationOutline,
      refreshOutline,
      saveOutline,
      trashOutline
    });

    this.formularioParqueo =
      this.formBuilder.group({
        nombreParqueo: [
          '',
          [
            Validators.required,
            Validators.maxLength(150)
          ]
        ],

        direccion: [
          '',
          [
            Validators.required,
            Validators.maxLength(250)
          ]
        ],

        telefono: [
          '',
          [
            Validators.maxLength(25)
          ]
        ],

        capacidadTotal: [
          1,
          [
            Validators.required,
            Validators.min(1)
          ]
        ],

        activo: [true]
      });

    this.formularioEspacio =
      this.formBuilder.group({
        numeroEspacio: [
          '',
          [
            Validators.required,
            Validators.maxLength(20)
          ]
        ],

        disponible: [true],
        activo: [true]
      });
  }

  ngOnInit(): void {
    this.cargarParqueos();
  }

  cargarParqueos(): void {
    this.cargandoParqueos = true;

    this.parqueoService
      .obtenerTodos()
      .subscribe({
        next: respuesta => {
          this.parqueos =
            respuesta.valorRetorno ?? [];

          this.filtrarParqueos();

          this.cargandoParqueos = false;

          if (this.parqueoSeleccionado) {
            const actualizado =
              this.parqueos.find(
                parqueo =>
                  parqueo.parqueoId ===
                  this.parqueoSeleccionado
                    ?.parqueoId
              );

            this.parqueoSeleccionado =
              actualizado ?? null;
          }
        },

        error: error => {
          this.cargandoParqueos = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible consultar los parqueos.';

          void this.mostrarError(mensaje);
        }
      });
  }

  filtrarParqueos(): void {
    const texto =
      this.textoBusquedaParqueo
        .trim()
        .toLowerCase();

    if (!texto) {
      this.parqueosFiltrados =
        [...this.parqueos];

      return;
    }

    this.parqueosFiltrados =
      this.parqueos.filter(parqueo =>
        parqueo.nombreParqueo
          .toLowerCase()
          .includes(texto) ||

        parqueo.direccion
          .toLowerCase()
          .includes(texto) ||

        (parqueo.telefono ?? '')
          .toLowerCase()
          .includes(texto)
      );
  }

  abrirFormularioParqueoNuevo(): void {
    this.modoEdicionParqueo = false;
    this.parqueoSeleccionadoId = null;

    this.formularioParqueo.reset({
      nombreParqueo: '',
      direccion: '',
      telefono: '',
      capacidadTotal: 1,
      activo: true
    });

    this.mostrarFormularioParqueo = true;
  }

  editarParqueo(
    parqueo: Parqueo
  ): void {
    this.modoEdicionParqueo = true;

    this.parqueoSeleccionadoId =
      parqueo.parqueoId;

    this.formularioParqueo.patchValue({
      nombreParqueo:
        parqueo.nombreParqueo,

      direccion:
        parqueo.direccion,

      telefono:
        parqueo.telefono ?? '',

      capacidadTotal:
        parqueo.capacidadTotal,

      activo:
        parqueo.activo
    });

    this.mostrarFormularioParqueo = true;

    window.scrollTo({
      top: 0,
      behavior: 'smooth'
    });
  }

  cancelarFormularioParqueo(): void {
    if (this.guardandoParqueo) {
      return;
    }

    this.mostrarFormularioParqueo = false;
    this.modoEdicionParqueo = false;
    this.parqueoSeleccionadoId = null;
  }

  guardarParqueo(): void {
    this.formularioParqueo
      .markAllAsTouched();

    if (this.formularioParqueo.invalid) {
      void this.mostrarAdvertencia(
        'Revise los datos obligatorios del parqueo.'
      );

      return;
    }

    this.guardandoParqueo = true;

    if (
      this.modoEdicionParqueo &&
      this.parqueoSeleccionadoId
    ) {
      this.actualizarParqueo();
      return;
    }

    this.crearParqueo();
  }

  seleccionarParqueo(
    parqueo: Parqueo
  ): void {
    this.parqueoSeleccionado = parqueo;

    this.cancelarFormularioEspacio();

    this.cargarEspacios(
      parqueo.parqueoId
    );
  }

  async desactivarParqueo(
    parqueo: Parqueo
  ): Promise<void> {

    if (!parqueo.activo) {
      await this.mostrarAdvertencia(
        'Este parqueo ya se encuentra inactivo.'
      );

      return;
    }

    const confirmacion =
      await Swal.fire({
        icon: 'warning',
        title: '¿Desactivar parqueo?',
        html:
          `Se desactivará <strong>${parqueo.nombreParqueo}</strong>.`,
        showCancelButton: true,
        confirmButtonText: 'Sí, desactivar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#21b9e8',
        cancelButtonColor: '#555b57',
        background: '#1c2325',
        color: '#f4f7f3'
      });

    if (!confirmacion.isConfirmed) {
      return;
    }

    this.parqueoService
      .eliminar(parqueo.parqueoId)
      .subscribe({
        next: respuesta => {
          void this.mostrarExito(
            'Parqueo desactivado',
            respuesta.mensaje
          );

          if (
            this.parqueoSeleccionado
              ?.parqueoId ===
            parqueo.parqueoId
          ) {
            this.parqueoSeleccionado = null;
            this.espacios = [];
            this.espaciosFiltrados = [];
          }

          this.cargarParqueos();
        },

        error: error => {
          const mensaje =
            error?.error?.mensaje ??
            'No fue posible desactivar el parqueo.';

          void this.mostrarError(mensaje);
        }
      });
  }

  cargarEspacios(
    parqueoId: number
  ): void {
    this.cargandoEspacios = true;

    this.espacioService
      .obtenerPorParqueo(parqueoId)
      .subscribe({
        next: respuesta => {
          this.espacios =
            respuesta.valorRetorno ?? [];

          this.filtrarEspacios();

          this.cargandoEspacios = false;
        },

        error: error => {
          this.cargandoEspacios = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible consultar los espacios.';

          void this.mostrarError(mensaje);
        }
      });
  }

  filtrarEspacios(): void {
    const texto =
      this.textoBusquedaEspacio
        .trim()
        .toLowerCase();

    if (!texto) {
      this.espaciosFiltrados =
        [...this.espacios];

      return;
    }

    this.espaciosFiltrados =
      this.espacios.filter(espacio =>
        espacio.numeroEspacio
          .toLowerCase()
          .includes(texto)
      );
  }

  abrirFormularioEspacioNuevo(): void {
    if (!this.parqueoSeleccionado) {
      void this.mostrarAdvertencia(
        'Primero debe seleccionar un parqueo.'
      );

      return;
    }

    if (!this.parqueoSeleccionado.activo) {
      void this.mostrarAdvertencia(
        'No puede agregar espacios a un parqueo inactivo.'
      );

      return;
    }

    this.modoEdicionEspacio = false;
    this.espacioSeleccionadoId = null;

    this.formularioEspacio.reset({
      numeroEspacio: '',
      disponible: true,
      activo: true
    });

    this.mostrarFormularioEspacio = true;
  }

  editarEspacio(
    espacio: EspacioParqueo
  ): void {
    this.modoEdicionEspacio = true;

    this.espacioSeleccionadoId =
      espacio.espacioId;

    this.formularioEspacio.patchValue({
      numeroEspacio:
        espacio.numeroEspacio,

      disponible:
        espacio.disponible,

      activo:
        espacio.activo
    });

    this.mostrarFormularioEspacio = true;
  }

  cancelarFormularioEspacio(): void {
    if (this.guardandoEspacio) {
      return;
    }

    this.mostrarFormularioEspacio = false;
    this.modoEdicionEspacio = false;
    this.espacioSeleccionadoId = null;
  }

  guardarEspacio(): void {
    this.formularioEspacio
      .markAllAsTouched();

    if (!this.parqueoSeleccionado) {
      void this.mostrarAdvertencia(
        'Debe seleccionar un parqueo.'
      );

      return;
    }

    if (this.formularioEspacio.invalid) {
      void this.mostrarAdvertencia(
        'Revise los datos obligatorios del espacio.'
      );

      return;
    }

    this.guardandoEspacio = true;

    if (
      this.modoEdicionEspacio &&
      this.espacioSeleccionadoId
    ) {
      this.actualizarEspacio();
      return;
    }

    this.crearEspacio();
  }

  async desactivarEspacio(
    espacio: EspacioParqueo
  ): Promise<void> {

    if (!espacio.activo) {
      await this.mostrarAdvertencia(
        'Este espacio ya se encuentra inactivo.'
      );

      return;
    }

    const confirmacion =
      await Swal.fire({
        icon: 'warning',
        title: '¿Desactivar espacio?',
        html:
          `Se desactivará el espacio <strong>${espacio.numeroEspacio}</strong>.`,
        showCancelButton: true,
        confirmButtonText: 'Sí, desactivar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#21b9e8',
        cancelButtonColor: '#555b57',
        background: '#1c2325',
        color: '#f4f7f3'
      });

    if (!confirmacion.isConfirmed) {
      return;
    }

    this.espacioService
      .eliminar(espacio.espacioId)
      .subscribe({
        next: respuesta => {
          void this.mostrarExito(
            'Espacio desactivado',
            respuesta.mensaje
          );

          this.recargarEspacios();
        },

        error: error => {
          const mensaje =
            error?.error?.mensaje ??
            'No fue posible desactivar el espacio.';

          void this.mostrarError(mensaje);
        }
      });
  }

  campoParqueoInvalido(
    campo: string
  ): boolean {
    const control =
      this.formularioParqueo.get(campo);

    return Boolean(
      control &&
      control.invalid &&
      control.touched
    );
  }

  campoEspacioInvalido(
    campo: string
  ): boolean {
    const control =
      this.formularioEspacio.get(campo);

    return Boolean(
      control &&
      control.invalid &&
      control.touched
    );
  }

  get totalParqueosActivos(): number {
    return this.parqueos.filter(
      parqueo => parqueo.activo
    ).length;
  }

  get capacidadTotal(): number {
    return this.parqueos
      .filter(parqueo => parqueo.activo)
      .reduce(
        (total, parqueo) =>
          total + parqueo.capacidadTotal,
        0
      );
  }

  get espaciosDisponibles(): number {
    return this.espacios.filter(
      espacio =>
        espacio.activo &&
        espacio.disponible
    ).length;
  }

  get espaciosOcupados(): number {
    return this.espacios.filter(
      espacio =>
        espacio.activo &&
        !espacio.disponible
    ).length;
  }

  private crearParqueo(): void {
    const valor =
      this.formularioParqueo.value;

    const parqueo: ParqueoCrear = {
      nombreParqueo:
        String(valor.nombreParqueo)
          .trim(),

      direccion:
        String(valor.direccion)
          .trim(),

      telefono:
        this.limpiarOpcional(
          valor.telefono
        ),

      capacidadTotal:
        Number(valor.capacidadTotal)
    };

    this.parqueoService
      .agregar(parqueo)
      .subscribe({
        next: respuesta => {
          this.guardandoParqueo = false;
          this.mostrarFormularioParqueo = false;

          void this.mostrarExito(
            'Parqueo registrado',
            respuesta.mensaje
          );

          this.cargarParqueos();
        },

        error: error => {
          this.guardandoParqueo = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible registrar el parqueo.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private actualizarParqueo(): void {
    if (!this.parqueoSeleccionadoId) {
      this.guardandoParqueo = false;
      return;
    }

    const valor =
      this.formularioParqueo.value;

    const parqueo:
      ParqueoActualizar = {

      nombreParqueo:
        String(valor.nombreParqueo)
          .trim(),

      direccion:
        String(valor.direccion)
          .trim(),

      telefono:
        this.limpiarOpcional(
          valor.telefono
        ),

      capacidadTotal:
        Number(valor.capacidadTotal),

      activo:
        Boolean(valor.activo)
    };

    this.parqueoService
      .actualizar(
        this.parqueoSeleccionadoId,
        parqueo
      )
      .subscribe({
        next: respuesta => {
          this.guardandoParqueo = false;
          this.mostrarFormularioParqueo = false;
          this.modoEdicionParqueo = false;
          this.parqueoSeleccionadoId = null;

          void this.mostrarExito(
            'Parqueo actualizado',
            respuesta.mensaje
          );

          this.cargarParqueos();
        },

        error: error => {
          this.guardandoParqueo = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible actualizar el parqueo.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private crearEspacio(): void {
    if (!this.parqueoSeleccionado) {
      this.guardandoEspacio = false;
      return;
    }

    const valor =
      this.formularioEspacio.value;

    const espacio:
      EspacioParqueoCrear = {

      parqueoId:
        this.parqueoSeleccionado
          .parqueoId,

      numeroEspacio:
        String(valor.numeroEspacio)
          .trim()
          .toUpperCase(),

      disponible:
        Boolean(valor.disponible)
    };

    this.espacioService
      .agregar(espacio)
      .subscribe({
        next: respuesta => {
          this.guardandoEspacio = false;
          this.mostrarFormularioEspacio = false;

          void this.mostrarExito(
            'Espacio registrado',
            respuesta.mensaje
          );

          this.recargarEspacios();
          this.cargarParqueos();
        },

        error: error => {
          this.guardandoEspacio = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible registrar el espacio.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private actualizarEspacio(): void {
    if (
      !this.parqueoSeleccionado ||
      !this.espacioSeleccionadoId
    ) {
      this.guardandoEspacio = false;
      return;
    }

    const valor =
      this.formularioEspacio.value;

    const espacio:
      EspacioParqueoActualizar = {

      parqueoId:
        this.parqueoSeleccionado
          .parqueoId,

      numeroEspacio:
        String(valor.numeroEspacio)
          .trim()
          .toUpperCase(),

      disponible:
        Boolean(valor.disponible),

      activo:
        Boolean(valor.activo)
    };

    this.espacioService
      .actualizar(
        this.espacioSeleccionadoId,
        espacio
      )
      .subscribe({
        next: respuesta => {
          this.guardandoEspacio = false;
          this.mostrarFormularioEspacio = false;
          this.modoEdicionEspacio = false;
          this.espacioSeleccionadoId = null;

          void this.mostrarExito(
            'Espacio actualizado',
            respuesta.mensaje
          );

          this.recargarEspacios();
        },

        error: error => {
          this.guardandoEspacio = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible actualizar el espacio.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private recargarEspacios(): void {
    if (!this.parqueoSeleccionado) {
      return;
    }

    this.cargarEspacios(
      this.parqueoSeleccionado.parqueoId
    );
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

  private async mostrarAdvertencia(
    mensaje: string
  ): Promise<void> {
    await Swal.fire({
      icon: 'warning',
      title: 'Revise la información',
      text: mensaje,
      confirmButtonColor: '#21b9e8',
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
      confirmButtonColor: '#21b9e8',
      background: '#1c2325',
      color: '#f4f7f3'
    });
  }
}