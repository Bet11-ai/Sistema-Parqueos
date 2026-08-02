import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import {
  IonBadge,
  IonButton,
  IonContent,
  IonHeader,
  IonIcon,
  IonInput,
  IonItem,
  IonLabel,
  IonSearchbar,
  IonSpinner,
  IonTitle,
  IonToggle,
  IonToolbar, IonList } from '@ionic/angular/standalone';

import { addIcons } from 'ionicons';
import {
  addOutline,
  arrowBackOutline,
  callOutline,
  checkmarkCircleOutline,
  closeCircleOutline,
  createOutline,
  mailOutline,
  peopleOutline,
  personOutline,
  refreshOutline,
  saveOutline,
  searchOutline,
  trashOutline
} from 'ionicons/icons';

import Swal from 'sweetalert2';

import {
  Cliente,
  ClienteActualizar,
  ClienteCrear
} from '../../models/cliente.model';

import { ClienteService } from '../../services/cliente.service';

@Component({
  selector: 'app-clientes',
  templateUrl: './clientes.page.html',
  styleUrls: ['./clientes.page.scss'],
  standalone: true,
  imports: [IonList, 
    CommonModule,
    FormsModule,
    IonHeader,
    IonToolbar,
    IonTitle,
    IonContent,
    IonButton,
    IonIcon,
    IonSearchbar,
    IonSpinner,
    IonBadge,
    IonItem,
    IonLabel,
    IonInput,
    IonToggle
  ]
})
export class ClientesPage implements OnInit {

  clientes: Cliente[] = [];
  clientesFiltrados: Cliente[] = [];

  textoBusqueda = '';
  cargando = false;
  guardando = false;
  mostrarFormulario = false;
  modoEdicion = false;

  clienteSeleccionadoId: number | null = null;

  formulario: ClienteActualizar = {
    nombre: '',
    apellidos: '',
    cedula: '',
    telefono: null,
    correo: null,
    activo: true
  };

  constructor(
    private readonly clienteService: ClienteService,
    private readonly router: Router
  ) {
    addIcons({
      addOutline,
      arrowBackOutline,
      callOutline,
      checkmarkCircleOutline,
      closeCircleOutline,
      createOutline,
      mailOutline,
      peopleOutline,
      personOutline,
      refreshOutline,
      saveOutline,
      searchOutline,
      trashOutline
    });
  }

  ngOnInit(): void {
    this.cargarClientes();
  }

  cargarClientes(): void {
    this.cargando = true;

    this.clienteService.obtenerTodos().subscribe({
      next: respuesta => {
        this.clientes = respuesta.valorRetorno ?? [];
        this.filtrarClientes();
        this.cargando = false;
      },
      error: error => {
        this.cargando = false;

        const mensaje =
          error?.error?.mensaje ??
          'No fue posible consultar los clientes.';

        void this.mostrarError(mensaje);
      }
    });
  }

  filtrarClientes(): void {
    const texto =
      this.textoBusqueda
        .trim()
        .toLowerCase();

    if (!texto) {
      this.clientesFiltrados = [...this.clientes];
      return;
    }

    this.clientesFiltrados =
      this.clientes.filter(cliente => {
        const nombreCompleto =
          `${cliente.nombre} ${cliente.apellidos}`
            .toLowerCase();

        return (
          nombreCompleto.includes(texto) ||
          cliente.cedula.toLowerCase().includes(texto) ||
          (cliente.correo ?? '')
            .toLowerCase()
            .includes(texto) ||
          (cliente.telefono ?? '')
            .toLowerCase()
            .includes(texto)
        );
      });
  }

  abrirFormularioNuevo(): void {
    this.modoEdicion = false;
    this.clienteSeleccionadoId = null;

    this.formulario = {
      nombre: '',
      apellidos: '',
      cedula: '',
      telefono: null,
      correo: null,
      activo: true
    };

    this.mostrarFormulario = true;
  }

  editarCliente(cliente: Cliente): void {
    this.modoEdicion = true;
    this.clienteSeleccionadoId =
      cliente.clienteId;

    this.formulario = {
      nombre: cliente.nombre,
      apellidos: cliente.apellidos,
      cedula: cliente.cedula,
      telefono: cliente.telefono,
      correo: cliente.correo,
      activo: cliente.activo
    };

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
    this.clienteSeleccionadoId = null;
  }

  guardarCliente(): void {
    this.normalizarFormulario();

    const errorValidacion =
      this.validarFormulario();

    if (errorValidacion) {
      void Swal.fire({
        icon: 'warning',
        title: 'Datos incompletos',
        text: errorValidacion,
        confirmButtonColor: '#667744',
        background: '#292b2b',
        color: '#f4f4f1'
      });

      return;
    }

    this.guardando = true;

    if (
      this.modoEdicion &&
      this.clienteSeleccionadoId
    ) {
      this.actualizarCliente();
      return;
    }

    this.crearCliente();
  }

  async desactivarCliente(
    cliente: Cliente
  ): Promise<void> {

    if (!cliente.activo) {
      await Swal.fire({
        icon: 'info',
        title: 'Cliente inactivo',
        text:
          'Este cliente ya se encuentra desactivado.',
        confirmButtonColor: '#667744',
        background: '#292b2b',
        color: '#f4f4f1'
      });

      return;
    }

    const confirmacion =
      await Swal.fire({
        icon: 'warning',
        title: '¿Desactivar cliente?',
        html:
          `Se desactivará a <strong>${cliente.nombre} ${cliente.apellidos}</strong>.`,
        showCancelButton: true,
        confirmButtonText: 'Sí, desactivar',
        cancelButtonText: 'Cancelar',
        confirmButtonColor: '#667744',
        cancelButtonColor: '#555b57',
        background: '#292b2b',
        color: '#f4f4f1'
      });

    if (!confirmacion.isConfirmed) {
      return;
    }

    this.clienteService
      .eliminar(cliente.clienteId)
      .subscribe({
        next: respuesta => {
          void Swal.fire({
            icon: 'success',
            title: 'Cliente desactivado',
            text: respuesta.mensaje,
            timer: 1400,
            showConfirmButton: false,
            background: '#292b2b',
            color: '#f4f4f1'
          });

          this.cargarClientes();
        },
        error: error => {
          const mensaje =
            error?.error?.mensaje ??
            'No fue posible desactivar el cliente.';

          void this.mostrarError(mensaje);
        }
      });
  }

  regresar(): void {
    void this.router.navigateByUrl(
      '/dashboard'
    );
  }

  get totalActivos(): number {
    return this.clientes.filter(
      cliente => cliente.activo
    ).length;
  }

  get totalInactivos(): number {
    return this.clientes.filter(
      cliente => !cliente.activo
    ).length;
  }

  private crearCliente(): void {
    const cliente: ClienteCrear = {
      nombre: this.formulario.nombre,
      apellidos: this.formulario.apellidos,
      cedula: this.formulario.cedula,
      telefono: this.formulario.telefono,
      correo: this.formulario.correo
    };

    this.clienteService
      .agregar(cliente)
      .subscribe({
        next: respuesta => {
          this.guardando = false;
          this.mostrarFormulario = false;

          void Swal.fire({
            icon: 'success',
            title: 'Cliente registrado',
            text: respuesta.mensaje,
            timer: 1400,
            showConfirmButton: false,
            background: '#292b2b',
            color: '#f4f4f1'
          });

          this.cargarClientes();
        },
        error: error => {
          this.guardando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible registrar el cliente.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private actualizarCliente(): void {
    if (!this.clienteSeleccionadoId) {
      this.guardando = false;
      return;
    }

    this.clienteService
      .actualizar(
        this.clienteSeleccionadoId,
        this.formulario
      )
      .subscribe({
        next: respuesta => {
          this.guardando = false;
          this.mostrarFormulario = false;
          this.modoEdicion = false;
          this.clienteSeleccionadoId = null;

          void Swal.fire({
            icon: 'success',
            title: 'Cliente actualizado',
            text: respuesta.mensaje,
            timer: 1400,
            showConfirmButton: false,
            background: '#292b2b',
            color: '#f4f4f1'
          });

          this.cargarClientes();
        },
        error: error => {
          this.guardando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible actualizar el cliente.';

          void this.mostrarError(mensaje);
        }
      });
  }

  private normalizarFormulario(): void {
    this.formulario.nombre =
      this.formulario.nombre.trim();

    this.formulario.apellidos =
      this.formulario.apellidos.trim();

    this.formulario.cedula =
      this.formulario.cedula.trim();

    this.formulario.telefono =
      this.limpiarOpcional(
        this.formulario.telefono
      );

    this.formulario.correo =
      this.limpiarOpcional(
        this.formulario.correo
      )?.toLowerCase() ?? null;
  }

  private validarFormulario():
    string | null {

    if (!this.formulario.nombre) {
      return 'El nombre es obligatorio.';
    }

    if (!this.formulario.apellidos) {
      return 'Los apellidos son obligatorios.';
    }

    if (!this.formulario.cedula) {
      return 'La cédula es obligatoria.';
    }

    if (
      this.formulario.correo &&
      !this.correoValido(
        this.formulario.correo
      )
    ) {
      return 'El correo electrónico no tiene un formato válido.';
    }

    return null;
  }

  private limpiarOpcional(
    valor: string | null
  ): string | null {

    const texto = valor?.trim();

    return texto
      ? texto
      : null;
  }

  private correoValido(
    correo: string
  ): boolean {

    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/
      .test(correo);
  }

  private async mostrarError(
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'error',
      title: 'Ocurrió un problema',
      text: mensaje,
      confirmButtonColor: '#667744',
      background: '#292b2b',
      color: '#f4f4f1'
    });
  }
}