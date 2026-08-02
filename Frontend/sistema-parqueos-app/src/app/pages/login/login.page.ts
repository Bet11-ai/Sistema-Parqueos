import { CommonModule } from '@angular/common';
import {
  Component
} from '@angular/core';
import {
  FormsModule
} from '@angular/forms';
import {
  Router
} from '@angular/router';

import {
  IonButton,
  IonContent,
  IonIcon,
  IonInput,
  IonItem,
  IonSpinner, IonHeader, IonToolbar, IonTitle } from '@ionic/angular/standalone';

import {
  addIcons
} from 'ionicons';
import {
  carSportOutline,
  eyeOffOutline,
  eyeOutline,
  lockClosedOutline,
  mailOutline
} from 'ionicons/icons';

import Swal from 'sweetalert2';

import {
  LoginSolicitud
} from '../../models/auth.model';
import {
  AuthService
} from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: true,
  imports: [IonTitle, IonToolbar, IonHeader, 
    CommonModule,
    FormsModule,
    IonContent,
    IonItem,
    IonInput,
    IonIcon,
    IonButton,
    IonSpinner
  ]
})
export class LoginPage {

  credenciales: LoginSolicitud = {
    correo: '',
    contrasena: ''
  };

  mostrarContrasena = false;
  procesando = false;

  constructor(
    private readonly authService:
      AuthService,
    private readonly router:
      Router
  ) {
    addIcons({
      carSportOutline,
      mailOutline,
      lockClosedOutline,
      eyeOutline,
      eyeOffOutline
    });
  }

  iniciarSesion(): void {
    if (
      !this.credenciales.correo.trim() ||
      !this.credenciales.contrasena
    ) {
      void Swal.fire({
        icon: 'warning',
        title: 'Datos incompletos',
        text:
          'Ingrese el correo y la contraseña.',
        confirmButtonColor: '#667744',
        background: '#292b2b',
        color: '#f4f4f1'
      });

      return;
    }

    this.procesando = true;

    this.authService
      .iniciarSesion({
        correo:
          this.credenciales.correo
            .trim()
            .toLowerCase(),

        contrasena:
          this.credenciales.contrasena
      })
      .subscribe({
        next: respuesta => {
          this.procesando = false;

          if (
            !respuesta.exito ||
            !respuesta.valorRetorno
          ) {
            void this.mostrarError(
              respuesta.mensaje
            );

            return;
          }

          void Swal.fire({
            icon: 'success',
            title: 'Bienvenida',
            text:
              `Hola, ${respuesta.valorRetorno.nombreCompleto}`,
            timer: 1300,
            showConfirmButton: false,
            background: '#292b2b',
            color: '#f4f4f1'
          }).then(() => {
            void this.router.navigateByUrl(
              '/dashboard',
              {
                replaceUrl: true
              }
            );
          });
        },

        error: error => {
          this.procesando = false;

          const mensaje =
            error?.error?.mensaje ??
            'No fue posible iniciar sesión.';

          void this.mostrarError(
            mensaje
          );
        }
      });
  }

  alternarContrasena(): void {
    this.mostrarContrasena =
      !this.mostrarContrasena;
  }

  private async mostrarError(
    mensaje: string
  ): Promise<void> {

    await Swal.fire({
      icon: 'error',
      title: 'Inicio de sesión fallido',
      text: mensaje,
      confirmButtonColor: '#667744',
      background: '#292b2b',
      color: '#f4f4f1'
    });
  }
}