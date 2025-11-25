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
  
  /*login(username: string, password: string): Observable<boolean> {
    // Validación básica
    if (!username?.trim() || !password?.trim()) {
      return throwError(() => new Error('Username and password are required'));
    }

    console.log('🔐 Attempting login for:', username);

    const loginData = { username, password };

    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData).pipe(
      tap(response => console.log('✅ Login response received')),
      switchMap(response => {
        // Validar respuesta
        if (!response?.encryptedPrivateKey || !response?.salt) {
          throw new Error('Invalid server response - missing encryption data');
        }

        this.saveToken(response.token);

        // Convertir promesa a observable
        return from(this.cryptoService.unlockPrivateKey(
          password,
          response.salt,
          response.encryptedPrivateKey
        )).pipe(
          map(privateKey => {
            const userProfile: RecieveAccount = {
              accountId: response.accountId,
              username: response.username,
              base64Pfp: response.base64Pfp,
              publicKey: response.publicKey,
              createdAt: new Date(response.createdAt),
              updatedAt: new Date(response.updatedAt)
            };

            this.stateService.setLoginSuccess(userProfile, { 
              userPrivateKey: privateKey 
            });
            
            console.log('✅ Login completed successfully');
            return true;
          })
        );
      }),
      catchError(error => {
        console.error('❌ Login failed:', error);
        this.logout();
        
        let userMessage = 'Login failed';
        if (error.status === 401) userMessage = 'Invalid credentials';
        if (error.status === 0) userMessage = 'Cannot connect to server';
        
        return throwError(() => new Error(userMessage));
      })
    );
  }*/
  
  login(username: string, password: string): Observable<boolean> {
    console.log('🔐 SIMPLE LOGIN - bypassing crypto');
    
    const loginData = { username, password };
    
    return this.http.post<LoginResponse>(`${this.apiUrl}/login`, loginData).pipe(
      map(response => {
        this.saveToken(response.token);
        
        const userProfile: RecieveAccount = {
          accountId: response.accountId,
          username: response.username,
          base64Pfp: response.base64Pfp,
          publicKey: response.publicKey,
          createdAt: new Date(response.createdAt),
          updatedAt: new Date(response.updatedAt)
        };
        
        // Usar una clave dummy temporal
        this.stateService.setLoginSuccess(userProfile, { 
          userPrivateKey: {} as CryptoKey 
        });
        
        return true;
      }),
      catchError(error => {
        console.error('Login error:', error);
        return throwError(() => new Error('Login failed'));
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