import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, from, throwError } from 'rxjs';
import { switchMap, tap, catchError, map } from 'rxjs/operators';

// Clases base y Servicios
import { APIdataReciever } from '../APIdataReciever';
import { EncryptationService } from '../Encryptation/encryptation.service';
import { StateService } from '../State/state.service'; // Asegúrate que la ruta sea correcta

// Interfaces
import { LoginResponse } from '../../app/interfaces/loginResponse';
import { RecieveAccount } from '../../app/interfaces/receiveAccount'; //

@Injectable({
  providedIn: 'root'
})
export class AuthService extends APIdataReciever {

  constructor(
    http: HttpClient,
    private cryptoService: EncryptationService,
    private stateService: StateService // <--- Inyectamos el State Real
  ) {
    super(http, "Account"); // Tu endpoint base
  }

  /**
   * 1. Pide credenciales al server
   * 2. Recibe PrivateKey cifrada + Salt
   * 3. Descifra la PrivateKey en el cliente
   * 4. Actualiza el StateService global
   */
  login(username: string, password: string): Observable<boolean> {
    const loginData = { username, password };

    // Paso 1: Llamada HTTP al backend (esperamos LoginResponse con salt y key cifrada)
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData).pipe(
      
      // Paso 2: Encadenamos el proceso de desencriptado
      switchMap(response => {
        // Guardamos JWT
        this.saveToken(response.token);

        // Convertimos la Promesa de desencriptado en un Observable
        return from(this.cryptoService.unlockPrivateKey(
          password,
          response.salt,
          response.encryptedPrivateKey
        )).pipe(
          // Paso 3: Si desencripta con éxito, actualizamos el Estado Global
          map(decryptedPrivateKey => {
            
            // A. Preparamos el objeto de usuario (Datos públicos)
            // Mapeamos la respuesta a tu interfaz RecieveAccount
            const userProfile: RecieveAccount = {
              accountId: response.accountId, // Asegúrate que tu LoginResponse tenga estos campos
              username: response.username,
              base64Pfp: response.base64Pfp,
              publicKey: response.publicKey,
              createdAt: response.createdAt,
              updatedAt: response.updatedAt
            };

            // B. Preparamos las llaves de sesión (Datos privados en memoria)
            const sessionKeys = {
              userPrivateKey: decryptedPrivateKey,
              // derivedPasswordKey: undefined // Opcional (ya le pusimos '?' en la interfaz)
            };

            // C. ¡ACTUALIZAMOS EL STATE SERVICE!
            // A partir de aquí, toda la app sabe que estás logueado y tiene tus llaves
            this.stateService.setLoginSuccess(userProfile, sessionKeys);

            return true; // Login exitoso
          })
        );
      }),
      
      // Manejo de errores (Credenciales mal o Password incorrecto al descifrar)
      catchError(err => {
        console.error("Login fallido:", err);
        this.logout(); // Limpiamos por si acaso
        return throwError(() => new Error('Credenciales inválidas o error de seguridad.'));
      })
    );
  }

  logout() {
    // 1. Borrar Token
    localStorage.removeItem(this.tokenKey);
    // 2. Limpiar el estado global (Borra las llaves de la memoria RAM)
    this.stateService.clearSession();
  }

  // --- Utilidades ---

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.tokenKey);
  }
  
  private saveToken(token: string) {
    localStorage.setItem(this.tokenKey, token);
  }
}